using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Cedar.CommandHandling.Http.Logging;
using NEventStore.Logging;
using TinyIoC;

namespace todoapp.overengineered.server.Infrastructure
{
    internal class TinyIocDependencyResolver : IDependencyResolver
    {
        private readonly TinyIoCContainer _container;

        public TinyIocDependencyResolver(TinyIoCContainer container)
        {
            _container = container;
        }

        public IDependencyScope BeginScope()
        {
            return new PerRequestScope(this);
        }

        public object GetService(Type serviceType)
        {
            if (_container.CanResolve(serviceType))
            {
                try
                {
                    return _container.Resolve(serviceType);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to create type {type} for Webapi controller", ex, serviceType.Name);
                    throw;
                }
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (_container.CanResolve(serviceType))
            {
                return _container.ResolveAll(serviceType, true);
            }
            return Enumerable.Empty<object>();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        /// <summary>
        /// Class that embodies a per request scope. We don't actualy use this, but webapi requires it.
        /// </summary>
        private class PerRequestScope : IDependencyScope
        {
            private readonly IDependencyResolver _resolver;

            public PerRequestScope(IDependencyResolver resolver)
            {
                _resolver = resolver;
            }


            public void Dispose()
            {
                // nothing to clean up at the end of each request. 
            }

            public object GetService(Type serviceType)
            {
                return _resolver.GetService(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return _resolver.GetServices(serviceType);
            }
        }
    }
}
