using Autofac;
using Inspector.POService.RequestHandler;
using System.Net.Http;

namespace Inspector.POService
{
    public static class Program
    {
        public static IContainer Container;
        public static void Main()
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(new HttpClient()).SingleInstance();

            builder.RegisterType<RequestHandler.RequestHandler>()
                .As<IRequestHandler>()
                .SingleInstance();

            Container = builder.Build();
        }
    }
}
