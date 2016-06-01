using System;
using System.Collections.Generic;

namespace todoapp.overengineered.server.Domain
{
    public abstract class AggregateRoot
    {
        private int _version;
        private readonly List<object> _events;
        private readonly IDictionary<Type, Action<object>> _handlers;
        public int Version => _version;

        public string Id { get; protected set; }

        public AggregateRoot()
        {
            _handlers = new Dictionary<Type, Action<object>>();
            _events = new List<object>();
        }

        protected void Register<T>(Action<T> apply)
        {
            _handlers[typeof(T)] = @event => apply((T)@event);
        }

        public IEnumerable<object> GetChanges()
        {
            return _events;
        }

        public void MarkChangesAsCommitted()
        {
            _events.Clear();
        }

        public void LoadFromHistory(IEnumerable<object> events)
        {
            foreach (var @event in events)
            {
                ApplyChange(@event, false);
            }
        }

        protected void ApplyChange(object @event, bool trackChanges = true)
        {
            Action<object> apply;

            if (_handlers.TryGetValue(@event.GetType(), out apply))
            {
                apply(@event);
            }

            _version++;

            if (trackChanges)
            {
                _events.Add(@event);
            }
        }
    }
}