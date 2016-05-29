using System;
using NEventStore.Client;
using NEventStore.Persistence.Sql.SqlDialects;
using NEventStore;
using NEventStore.Dispatcher;
using todoapp.overengineered.server.Messages;
using todoapp.overengineered.server.Projections;
using TinyIoC;
using static NEventStore.Wireup;

namespace todoapp.overengineered.server.Infrastructure
{
    public class EventStoreBootstrapper
    {
        private readonly Projector _projector;

        public EventStoreBootstrapper(Projector projector)
        {
            _projector = projector;
        }

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


            var pollingClient = new PollingClient(store.Advanced);
            string checkpointToken = LoadCheckpoint();

            IObserveCommits observeCommits = pollingClient.ObserveFrom(checkpointToken);
            observeCommits.Subscribe(new ReadModelCommitObserver(new DelegateMessageDispatcher(DispatchCommit)));
            observeCommits.Start();
            SaveCheckpoint(checkpointToken);

            return store;
        }

        private static string LoadCheckpoint()
        {
            // Load the checkpoint value from disk / local db/ etc
            return null;
        }

        private static void SaveCheckpoint(string checkpointToken)
        {
            //Save checkpointValue to disk / whatever.
        }

        private void DispatchCommit(ICommit commit)
        {
            try
            {
                foreach (EventMessage @event in commit.Events)
                {
                    if (@event.Body is TodoCreated)
                    {
                        _projector.Project((TodoCreated)@event.Body);
                    }
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to dispatch");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Checkpoint token= " + commit.CheckpointToken);
        }

        public class ReadModelCommitObserver : IObserver<ICommit>
        {
            private readonly IDispatchCommits dispatcher;

            public ReadModelCommitObserver(IDispatchCommits dispatcher)
            {
                this.dispatcher = dispatcher;
            }

            public void OnCompleted()
            {
                Console.WriteLine("commit observation completed.");
            }

            public void OnError(Exception error)
            {
                Console.WriteLine("Exception from ReadModelCommitObserver: {0}", error.Message);
                throw error;
            }

            public void OnNext(ICommit commit)
            {
                dispatcher.Dispatch(commit);
            }
        }
    }
}
