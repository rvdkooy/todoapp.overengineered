using System;
using System.Collections.Generic;

namespace todoapp.overengineered.server.Projections
{
    public class Projector
    {
        private readonly IDictionary<Type, Action<object>> _registrations = new Dictionary<Type, Action<object>>();

        public void When<T>(Action<T> action)
        {
            _registrations.Add(typeof(T), e => action.Invoke((T)e) );
        }

        public void Project(object @event)
        {
            var eventType = @event.GetType();

            if (_registrations.ContainsKey(eventType))
            {
                var action = _registrations[eventType];
                action.Invoke(@event);
            }
        }
    }
}
