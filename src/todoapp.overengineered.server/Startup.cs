using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using Cedar.CommandHandling;
using Cedar.CommandHandling.Http;
using Cedar.CommandHandling.Http.TypeResolution;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using NEventStore;
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
            var eventStoreBootstrapper = new EventStoreBootstrapper();
            var eventStore = eventStoreBootstrapper.Start();

            container.Register(new EventStoreRepository(eventStore));
            container.Register(inMemoryProjectionStore);

            RegisterProjections(eventStore, inMemoryProjectionStore);
            RegisterMiddleWare(app, container);
        }

        private void RegisterProjections(IStoreEvents eventStore, InMemoryProjectionStore inMemoryProjectionStore)
        {
            var eventStorePoller = new EventStorePoller(eventStore, ProjectionsModule.GetProjectors(inMemoryProjectionStore));
            eventStorePoller.Start();
        }

        private void RegisterMiddleWare(IAppBuilder app, TinyIoCContainer container)
        {
            var commandModule = container.Resolve<CommandModule>();
            var resolver = new CommandHandlerResolver(commandModule);

            
            var middleWare = CommandHandlingMiddleware.HandleCommands(new CommandHandlingSettings(
                resolver,
                UsingDataContractNamespace(resolver.KnownCommandTypes)
                ));

            app.Map("/commands", builder => builder.Use(middleWare));
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem($@"{Environment.CurrentDirectory}\..\..\..\wwwroot"),
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

        public static CommandMediaTypeMap UsingDataContractNamespace(IEnumerable<Type> knownCommandTypes)
        {
            var map = new CommandMediaTypeMap(new CommandMediaTypeWithDotVersionFormatter());

            foreach (var type in knownCommandTypes)
            {
                map.Add(type.Name.ToLower(), type);
            }

            return map;
        }
    }
}