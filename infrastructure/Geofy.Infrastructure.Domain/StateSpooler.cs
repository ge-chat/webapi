using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Geofy.Infrastructure.Domain.Transitions;
using Geofy.Infrastructure.ServiceBus.Dispatching;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.Domain
{
    public class StateSpooler
    {
        private static readonly ConcurrentDictionary<MethodDescriptor, MethodInfo> _methodCache = new ConcurrentDictionary<MethodDescriptor, MethodInfo>();

        public static void Spool(AggregateState state, IEvent evnt)
        {
            if (state == null) throw new ArgumentNullException("state");
            InvokeMethodOn(state, evnt);
        }

        public static void Spool(AggregateState state, IEnumerable<IEvent> events)
        {
            if (state == null) throw new ArgumentNullException("state");

            foreach (var evnt in events)
                InvokeMethodOn(state, evnt);
        }

        private static void InvokeMethodOn(AggregateState state, IEvent evnt)
        {
            state.Invoke(evnt);
        }

        public static void Spool(AggregateState state, IEnumerable<Transition> transitions)
        {
            Spool(state, transitions.SelectMany(t => t.Events).Select(e => (IEvent) e.Data));
        }      
    }
}