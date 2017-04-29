﻿using System;
using System.Collections.Generic;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.Domain
{
    public class AggregateState
    {
        private readonly Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        protected virtual AggregateState On<TMessage>(Action<TMessage> handler) where TMessage : IEvent
        {
            _handlers.Add(typeof(TMessage), o =>
            {
                var message = (TMessage)o;
                handler(message);
            });
            return this;
        }

        public void Invoke(object message)
        {
            var type = message.GetType();
            if (_handlers.ContainsKey(type))
            {
                _handlers[message.GetType()](message);
            }
        }
    }
}