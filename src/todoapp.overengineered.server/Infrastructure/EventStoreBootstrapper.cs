using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore;
using static NEventStore.Wireup;

namespace todoapp.overengineered.server.Infrastructure
{
    public class EventStoreBootstrapper
    {
        public IStoreEvents Start()
        {
            IStoreEvents store = Init()
                .LogToConsoleWindow()
                .UsingSqlPersistence("defaultConnectionString")
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .Compress()
                .Build();
            return store;
        }
    }
}
