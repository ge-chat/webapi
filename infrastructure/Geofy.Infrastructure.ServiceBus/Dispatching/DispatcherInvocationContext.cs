using System;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.Dispatching
{
    public sealed class DispatcherInvocationContext
    {
        private readonly Dispatching.Dispatcher _dispatcher;
        private readonly object _handler;
        private readonly object _message;

        public object Message
        {
            get { return _message; }
        }

        public object Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherInvocationContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherInvocationContext(Dispatching.Dispatcher dispatcher, Object handler, Object message)
        {
            _dispatcher = dispatcher;
            _handler = handler;
            _message = message;
        }

        public Task InvokeAsync()
        {
            return _dispatcher.InvokeByReflectionAsync(_handler, _message);
        }
    }

    public class DispatcherInterceptorContext
    {
        private readonly IMessageHandlerInterceptor _interceptor;
        private readonly DispatcherInvocationContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherInterceptorContext(IMessageHandlerInterceptor interceptor, DispatcherInvocationContext context)
        {
            _interceptor = interceptor;
            _context = context;
        }

        public void Invoke()
        {
            _interceptor.Intercept(_context);
        }
    }
}