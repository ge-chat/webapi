using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Exceptions;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using ILogFactory = Geofy.Infrastructure.ServiceBus.Logging.ILogFactory;
using ILoggingService = Geofy.Infrastructure.ServiceBus.Logging.ILoggingService;

namespace Geofy.Infrastructure.ServiceBus.Dispatching
{
    public class Dispatcher : IDispatcher
    {
        private ILoggingService _log;

        /// <summary>
        /// Service Locator that is used to create handlers
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Registry of all registered handlers
        /// </summary>
        private readonly DispatcherHandlerRegistry _registry;

        /// <summary>
        /// Number of retries in case exception was logged
        /// </summary>
        private readonly int _maxRetries;


        /// <summary>
        /// Current message that is dispatched 
        /// </summary>
        [ThreadStatic]
        public static Object CurrentMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Dispatcher(DispatcherConfiguration configuration, ILogFactory loggingFactory)
        {
            _log = loggingFactory.GetLoggingService(ToString());
            if (configuration.ServiceProvider == null)
                throw new ArgumentException("Unity Container is not registered for distributor.");

            if (configuration.DispatcherHandlerRegistry == null)
                throw new ArgumentException("Dispatcher Handler Registry is null in distributor.");

            _serviceProvider = configuration.ServiceProvider;
            _registry = configuration.DispatcherHandlerRegistry;
            _maxRetries = configuration.NumberOfRetries;

            // order handlers 
            _registry.InsureOrderOfHandlers();
        }

        /// <summary>
        /// Factory method
        /// </summary>
        public static Dispatcher Create(Func<DispatcherConfiguration, DispatcherConfiguration> configurationAction, ILogFactory loggingFactory)
        {
            var config = new DispatcherConfiguration();
            configurationAction(config);
            return new Dispatcher(config, loggingFactory);
        }

        public Task DispatchAsync(Object message)
        {
            return DispatchAsync(message, null);
        }

        public async Task DispatchAsync(Object message, Action<Exception> exceptionObserver)
        {
            try
            {
                CurrentMessage = message;

                var subscriptions = _registry.GetSubscriptions(message.GetType());

                foreach (var subscription in subscriptions)
                {
                    var handler = _serviceProvider.GetService(subscription.HandlerType);

                    try
                    {
                        await ExecuteHandler(handler, message, exceptionObserver);
                    }
                    catch (HandlerException handlerException)
                    {
                        _log.Error(handlerException, "{0}", "Message handling failed.");
                    }
                }
            }
            catch (Exception exception)
            {
                _log.Error(exception, "{0}", "Error when dispatching message");
            }
        }

        private async Task ExecuteHandler(Object handler, Object message, Action<Exception> exceptionObserver = null)
        {
            var attempt = 0;
            while (attempt < _maxRetries)
            {
                try
                {
                    var context = new DispatcherInvocationContext(this, handler, message);

                    if (_registry.Interceptors.Count > 0)
                    {
                        // Call interceptors in backward order
                        for (int i = _registry.Interceptors.Count - 1; i >= 0; i--)
                        {
                            var interceptorType = _registry.Interceptors[i];
                            var interceptor = (IMessageHandlerInterceptor)_serviceProvider.GetService(interceptorType);
                            var interceptorContext = new DispatcherInterceptorContext(interceptor, context);
                            interceptorContext.Invoke();
                        }
                    }

                    await context.InvokeAsync();

                    // message handled correctly - so that should be 
                    // the final attempt
                    attempt = _maxRetries;
                }
                catch (Exception exception)
                {
                    exceptionObserver?.Invoke(exception);

                    attempt++;

                    if (attempt == _maxRetries)
                    {
                        throw new HandlerException(
                            $"Exception in the handler {handler.GetType().FullName} for message {message.GetType().FullName}", exception, message);

                    }
                }
            }            
        }

        public void InvokeDynamic(Object handler, Object message)
        {
            dynamic dynamicHandler = handler;
            dynamic dynamicMessage = message;

            dynamicHandler.Handle(dynamicMessage);
        }

        private readonly ConcurrentDictionary<MethodDescriptor, MethodInfo> _methodCache = new ConcurrentDictionary<MethodDescriptor, MethodInfo>();

        public async Task InvokeByReflectionAsync(Object handler, Object message)
        {
            var methodDescriptor = new MethodDescriptor(handler.GetType(), message.GetType());
            MethodInfo methodInfo = null;
            if (!_methodCache.TryGetValue(methodDescriptor, out methodInfo))
                _methodCache[methodDescriptor] = methodInfo = handler.GetType().GetMethod("HandleAsync", new[] { message.GetType() });

            if (methodInfo == null)
                return;

            await (Task)methodInfo.Invoke(handler, new[] { message });
        }
    }

    public struct MethodDescriptor
    {
        public readonly Type HandlerType;
        public readonly Type MessageType;

        public MethodDescriptor(Type handlerType, Type messageType) : this()
        {
            HandlerType = handlerType;
            MessageType = messageType;
        }

        public bool Equals(MethodDescriptor descriptor)
        {
            return descriptor.HandlerType == HandlerType && descriptor.MessageType == MessageType;
        }

        public override bool Equals(object descriptor)
        {
            if (ReferenceEquals(null, descriptor))
                return false;

            if (descriptor.GetType() != typeof(MethodDescriptor))
                return false;

            return Equals((MethodDescriptor) descriptor);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HandlerType != null ? HandlerType.GetHashCode() : 0) * 397) 
                     ^ (MessageType != null ? MessageType.GetHashCode() : 0);
            }
        }
    }

}
