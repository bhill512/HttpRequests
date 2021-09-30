using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpRequests
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var services = new ServiceCollection().AddHttpClient();
            var builder = new ContainerBuilder();
            builder.Populate(services);
            var container = builder.Build();
            var factory = container.Resolve<IHttpClientFactory>();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .File(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Http_Requests_Logs.txt"))
                .CreateLogger();

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"),
                    optional: false);

            if (args?.Length > 0 && args[0] == "dev")
            {
                configBuilder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "appsettings.dev.json"), optional: true);
            }

            IConfiguration config = configBuilder.Build();

            var client = new Client(Log.Logger, factory);
            var displayInfo = new DisplayInfo(Log.Logger);

            Console.WriteLine("Please input an IMDB Id: \n eg: 0213338 or 0090506");
            var imdbId = Console.ReadLine();

            var imdbInfo = await client.GetShowLookupResponse($"{config["TvMazeRootUrl"]}", imdbId)
                .ConfigureAwait(false);

            displayInfo.DisplayImdbInfo(imdbInfo);
        }
    }
}
