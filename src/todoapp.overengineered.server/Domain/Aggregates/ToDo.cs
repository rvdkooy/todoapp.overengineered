using System;
using todoapp.overengineered.server.Messages;

namespace todoapp.overengineered.server.Domain.Aggregates
{
    public class ToDo : AggregateRoot
    {
        public ToDo(string text)
        {
            Register<TodoCreated>((e) =>
            {
                Id = e.Id;
            });

            ApplyChange(new TodoCreated(Guid.NewGuid().ToString(), text));
        }
    }
}
