using todoapp.overengineered.server.Messages;

namespace todoapp.overengineered.server.Projections
{
    public class Projector
    {
        private readonly InMemoryProjectionStore _projectionStore;

        public Projector(InMemoryProjectionStore projectionStore)
        {
            _projectionStore = projectionStore;
        }

        public void Project(TodoCreated todoCreated)
        {
            _projectionStore.Add(todoCreated.Id, new
            {
                Id = todoCreated.Id,
                Text = todoCreated.Text
            });
        }
    }
}
