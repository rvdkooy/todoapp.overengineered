using System;
using todoapp.overengineered.server.Messages;

namespace todoapp.overengineered.server.Domain.Aggregates
{
    public class ToDo : AggregateRoot
    {
        public ToDo()
        {
            Register<TodoCreated>((e) => Id = e.Id);
            Register<TodoDeleted>((e) => IsDeleted = true);
        }

        public ToDo(string text) : this()
        {
            ApplyChange(new TodoCreated(Guid.NewGuid().ToString(), text));
        }

        public void Delete()
        {
            ApplyChange(new TodoDeleted(Id));
        }

        public bool IsDeleted { get; private set; }
    }
}
