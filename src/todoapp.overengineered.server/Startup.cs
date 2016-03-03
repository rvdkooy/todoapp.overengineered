using System.Threading.Tasks;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace todoapp.overengineered.server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/statics", builder =>
            {
                builder.UseFileServer(new FileServerOptions
                {
                    FileSystem =
                        new PhysicalFileSystem(@"C:\DEV\todoapp.overengineered\src\todoapp.overengineered.server")
                });
            });
        }
    }
}