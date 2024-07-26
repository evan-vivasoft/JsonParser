using Autofac;
using JSONParser.RequestHandler;
using JSONParser.StationInformation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser
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
