namespace todoapp.overengineered.server.Messages
{
    public class TodoDeleted
    {
        public TodoDeleted(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}