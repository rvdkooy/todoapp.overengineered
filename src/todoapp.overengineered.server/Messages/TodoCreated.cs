
namespace todoapp.overengineered.server.Messages
{
    public class TodoCreated
    {
        public TodoCreated(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public string Id { get; set; }
        public string Text { get; }
    }
}