using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cedar.CommandHandling;
using todoapp.overengineered.server.Commanding.Commands;

namespace todoapp.overengineered.server.Commanding
{
    class CommandModule : CommandHandlerModule
    {
        public CommandModule()
        {
            For<NewTodo>().Handle(commandMessage =>
            {
                // write to an eventstore here!
            });
        }
    }
}
