﻿using Cedar.CommandHandling;
using todoapp.overengineered.server.Commanding.Commands;
using todoapp.overengineered.server.Domain.Aggregates;
using todoapp.overengineered.server.Infrastructure;

namespace todoapp.overengineered.server.Commanding
{
    class CommandModule : CommandHandlerModule
    {
        private readonly EventStoreRepository _eventStoreRepository;

        public CommandModule(EventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;

            For<NewTodo>().Handle(commandMessage =>
            {
                var todo = new ToDo(commandMessage.Command.Text);
                _eventStoreRepository.Create(todo);
            });

            For<DeleteTodo>().Handle(commandMessage =>
            {
                var toDo = _eventStoreRepository.Get(commandMessage.Command.Id, new ToDo());

                toDo.Delete();

                _eventStoreRepository.Update(toDo);
            });
        }
    }
}
