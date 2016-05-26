using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cedar.CommandHandling;
using Cedar.CommandHandling.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using todoapp.overengineered.server.Commanding;
using todoapp.overengineered.server.Commanding.Commands;

namespace todoapp.overengineered.server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var resolver = new CommandHandlerResolver(new CommandModule());
            var middleWare = CommandHandlingMiddleware.HandleCommands(new CommandHandlingSettings(
                resolver, 
                ResolveCommandType
            ));

            app.Map("/commands", builder => builder.Use(middleWare));
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(string.Format(@"{0}\..\..\..\wwwroot", Environment.CurrentDirectory)),
                EnableDefaultFiles = true
            });
        }

        private Type ResolveCommandType(string mediaType)
        {
            return typeof(NewTodo);
        }
    }
}