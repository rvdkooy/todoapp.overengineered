using System;
using NEventStore;
using todoapp.overengineered.server.Domain;

namespace todoapp.overengineered.server.Infrastructure
{
    public class EventStoreRepository
    {
        private readonly IStoreEvents _store;
        public EventStoreRepository(IStoreEvents store)
        {
            _store = store;
        }

        public void Save(AggregateRoot aggregate)
        {
            using (var stream = _store.CreateStream(aggregate.Id))
            {
                foreach (var @event in aggregate.GetChanges())
                {
                    stream.Add(new EventMessage { Body = @event });
                }

                stream.CommitChanges(Guid.NewGuid());

                aggregate.MarkChangesAsCommitted();
            }
        }
    }
}
