using System;
using System.Collections.Generic;
using NEventStore;
using NEventStore.Client;
using NEventStore.Dispatcher;
using todoapp.overengineered.server.Projections;

namespace todoapp.overengineered.server.Infrastructure
{
    public class EventStorePoller
    {
        private readonly IStoreEvents _eventStore;
        private readonly IEnumerable<Projector> _projectors;

        public EventStorePoller(IStoreEvents eventStore, IEnumerable<Projector> projectors)
        {
            _eventStore = eventStore;
            _projectors = projectors;
        }

        public void Start()
        {
            var pollingClient = new PollingClient(_eventStore.Advanced, 1000);
            string checkpointToken = LoadCheckpoint();

            IObserveCommits observeCommits = pollingClient.ObserveFrom(checkpointToken);
            observeCommits.Subscribe(new ReadModelCommitObserver(new DelegateMessageDispatcher(DispatchCommit)));
            observeCommits.Start();
            SaveCheckpoint(checkpointToken);
        }

        private void DispatchCommit(ICommit commit)
        {
            try
            {
                foreach (EventMessage @event in commit.Events)
                {
                    foreach (var projector in _projectors)
                    {
                        projector.Project(@event.Body);
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
            private readonly IDispatchCommits _dispatcher;

            public ReadModelCommitObserver(IDispatchCommits dispatcher)
            {
                _dispatcher = dispatcher;
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
                _dispatcher.Dispatch(commit);
            }
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
    }
}
