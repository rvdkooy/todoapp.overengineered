using System.Collections.Generic;
using todoapp.overengineered.server.Messages;

namespace todoapp.overengineered.server.Projections
{
    public static class ProjectionsModule
    {
        internal static IEnumerable<Projector> GetProjectors(InMemoryProjectionStore projectionStore)
        {
            return new[]
            {
                new TodoProjector(projectionStore), 
            };
        }
    }

    class TodoProjector : Projector
    {
        public TodoProjector(InMemoryProjectionStore projectionStore)
        {
            When<TodoCreated>(@event =>
            {
                projectionStore.Add(@event.Id, new
                {
                    @event.Id,
                    @event.Text
                });
            });

            When<TodoDeleted>(@event =>
                projectionStore.Delete(@event.Id)
            );
        }
    }
}
