using System;
using System.Linq;
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

        public void Create(AggregateRoot aggregate)
        {
            try
            {
                using (var stream = _store.CreateStream(aggregate.Id))
                {
                    AppendToStreamAndCommit(aggregate, stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void Update(AggregateRoot aggregate)
        {
            try
            {
                using (var stream = _store.OpenStream(aggregate.Id))
                {
                    AppendToStreamAndCommit(aggregate, stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void AppendToStreamAndCommit(AggregateRoot aggregate, IEventStream stream)
        {
            foreach (var @event in aggregate.GetChanges())
            {
                stream.Add(new EventMessage { Body = @event });
            }

            stream.CommitChanges(Guid.NewGuid());

            aggregate.MarkChangesAsCommitted();
        }

        public T Get<T>(string id, T instance) where T : AggregateRoot
        {
            using (var stream = _store.OpenStream(id))
            {
                instance.LoadFromHistory(stream.CommittedEvents.Select(e => e.Body));
                return instance;
            }
        }
    }
}
