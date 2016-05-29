using System;
using System.Web.Http;
using Cedar.CommandHandling;
using Cedar.CommandHandling.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using todoapp.overengineered.server.Commanding;
using todoapp.overengineered.server.Commanding.Commands;
using todoapp.overengineered.server.Infrastructure;
using todoapp.overengineered.server.Projections;
using TinyIoC;

namespace todoapp.overengineered.server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new TinyIoCContainer();
            var inMemoryProjectionStore = new InMemoryProjectionStore();
            var projector = new Projector(inMemoryProjectionStore);
            var eventStoreBootstrapper = new EventStoreBootstrapper(projector);
            var store = eventStoreBootstrapper.Start();

            container.Register(new EventStoreRepository(store));
            container.Register(inMemoryProjectionStore);
            
            registerMiddleWare(app, container);
        }

        private void registerMiddleWare(IAppBuilder app, TinyIoCContainer container)
        {
            var commandModule = container.Resolve<CommandModule>();
            var resolver = new CommandHandlerResolver(commandModule);
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


            var dependencyResolver = new TinyIocDependencyResolver(container);
            var config = new HttpConfiguration
            {
                DependencyResolver = dependencyResolver
            };
            config.MapHttpAttributeRoutes();
            app.Map("/api", builder => builder.UseWebApi(config));
        }

        private Type ResolveCommandType(string mediaType)
        {
            if (mediaType.ToLower().Contains("addtodo"))
            {
                return typeof(NewTodo);
            }

            throw new InvalidOperationException("Could not resolve commandtype: " + mediaType);
        }
    }
}