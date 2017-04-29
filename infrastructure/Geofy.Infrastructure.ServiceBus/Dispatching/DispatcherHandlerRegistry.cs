﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Geofy.Infrastructure.ServiceBus.Dispatching.Attributes;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Infrastructure.ServiceBus.Dispatching.Utils;

namespace Geofy.Infrastructure.ServiceBus.Dispatching
{
    public enum NamespaceComparison
    {
        Include, Exclude
    }

    public class DispatcherHandlerRegistry
    {
        private const string HandleMethodName = "HandleAsync";
        private readonly Type _markerInterfaceGeneric = typeof(IMessageHandlerAsync<>);
        private readonly Type _markerInterface = typeof(IMessageHandler);

        /// <summary>
        /// Message type -> List of handlers type
        /// </summary>
        private readonly Dictionary<Type, List<Subscription>> _subscription = new Dictionary<Type, List<Subscription>>();

        /// <summary>
        /// Message interceptors
        /// </summary>
        private readonly List<Type> _interceptors = new List<Type>();

        /// <summary>
        /// Message interceptors
        /// </summary>
        public List<Type> Interceptors
        {
            get { return _interceptors; }
        }

        /// <summary>
        /// Register all handlers in assembly (you can register handlers that optionally belongs to specified namespaces)
        /// </summary>
        public void Register(Assembly assembly, String[] namespaces, NamespaceComparison comparison = NamespaceComparison.Include)
        {
            var searchTarget = _markerInterfaceGeneric;

            foreach (var type in assembly.GetTypes())
            {
                if (!BelongToNamespaces(type, namespaces, comparison))
                    continue;

                var priorityAttribute = ReflectionUtils.GetSingleAttribute<PriorityAttribute>(type);
                var defaultPriority = priorityAttribute == null ? 0 : priorityAttribute.Priority;

                var interfaces = type.GetInterfaces();
                var markerInterface = interfaces.FirstOrDefault(i => i == _markerInterface);
                var markerInterfacesGeneric = interfaces.Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == searchTarget) && !i.ContainsGenericParameters).ToList();

                if (markerInterface != null)
                {
                    var methods = type.GetMethods()
                        .Select(m => new {Method = m, Parameters = m.GetParameters()})
                        .Where(m => m.Method.ReturnType == typeof(void) && m.Parameters.Count() == 1);

                    foreach (var method in methods)
                    {
                        if (method.Method.Name != HandleMethodName)
                        {
                            throw new Exception(string.Format(
                                "Handler '{0}' has incorrect name of method '{1}' for handling message '{2}'. Change method name to '{3}' or make it not-public (change return type, parameters count, make class not-inhereted from {4}, etc.) to prevent it from message handling.", 
                                type, method.Method.Name, method.Parameters.First().ParameterType, HandleMethodName, markerInterface));
                        }

                        var methodAttribute = ReflectionUtils.GetSingleAttribute<PriorityAttribute>(method.Method);
                        var finalPriority = methodAttribute == null ? defaultPriority : methodAttribute.Priority;

                        if (method.Parameters.Count() != 1)
                            continue;

                        var messageType = method.Parameters[0].ParameterType;
                        AddSubscription(messageType, type, finalPriority);
                    }
                    
                }
                else if (markerInterfacesGeneric.Count > 0)
                {
                    // for generic marker interfaces [Priority] attribute not supported for methods, but supported for class
                    foreach (var i in markerInterfacesGeneric)
                    {
                        var messageType = i.GetGenericArguments()[0];
                        AddSubscription(messageType, type, defaultPriority);
                    }
                }
            }
        }

        private void AddSubscription(Type messageType, Type handlerType, Int32 priority)
        {
            if (!_subscription.ContainsKey(messageType))
                _subscription[messageType] = new List<Subscription>();

            var subscription = new Subscription(handlerType, priority);

            if (!_subscription[messageType].Contains(subscription))
                _subscription[messageType].Add(subscription);
        }

        public void InsureOrderOfHandlers()
        {
            foreach (var type in _subscription.Keys)
            {
                var handlerTypes = _subscription[type];
                SortInPlace(handlerTypes);
            }
        }

        public void SortInPlace(List<Subscription> list)
        {
            list.Sort((sub1, sub2) =>
            {
                if (sub1.Priority == sub2.Priority)
                    return 0;

                return (sub1.Priority < sub2.Priority) ? -1 : 1;
            });
        }

        public void AddInterceptor(Type type)
        {
            if (!typeof(IMessageHandlerInterceptor).IsAssignableFrom(type))
                throw new Exception(String.Format("Interceptor {0} must implement IMessageHandlerInterceptor", type.FullName));

            if (_interceptors.Contains(type))
                throw new Exception(String.Format("Interceptor {0} already registered", type.FullName));

            _interceptors.Add(type);
        }

        private Boolean BelongToNamespaces(Type type, String[] namespaces, NamespaceComparison comparison)
        {
            // if no namespaces specified - then type belong to any namespace
            if (namespaces.Length == 0)
                return true;

            if (comparison == NamespaceComparison.Include)
            {
                foreach (var ns in namespaces)
                {
                    if (type.FullName.StartsWith(ns))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (var ns in namespaces)
                {
                    if (type.FullName.StartsWith(ns))
                    {
                        return false;
                    }

                }
                return true;
            }

        }

        public List<Subscription> GetSubscriptions(Type messageType)
        {
            if (!_subscription.ContainsKey(messageType))
                return new List<Subscription>();

            var handlers = _subscription[messageType];

            if (handlers.Count < 1)
            {
                string errorMessage = String.Format("Handler for type {0} doesn't found.", messageType.FullName);
                throw new Exception(errorMessage);
            }

            return handlers;
        }
    }
}
