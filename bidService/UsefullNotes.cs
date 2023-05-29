/*using AuctionTrackerWorker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Core;
using AuctionTrackerWorker.Services;
using AuctionTrackerWorker.Controllers;
using NLog;
using NLog.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();

        services.AddSingleton<IMongoServiceFactory>(provider =>
        {
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            return new MongoServiceFactory(scopeFactory);
        });

        // Add MVC services
        //services.AddMvc();
        services.AddScoped<IMongoService, MongoService>();
        services.AddMvc().AddApplicationPart(typeof(CRUDController).Assembly);
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure(app =>
        {
            // Add MVC middleware
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                // Map your MVC routes
                endpoints.MapControllers();
                LogMappedControllers(endpoints);
            });
        });
    })
    .UseNLog()
    .Build();

host.Run();

void LogMappedControllers(IEndpointRouteBuilder endpoints)
{
    var mappedControllers = endpoints.DataSources
        .Where(ds => ds.GetType().FullName == "Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionEndpointDataSource")
        .SelectMany(ds => ds.Endpoints)
        .OfType<RouteEndpoint>()
        .Where(re => re.Metadata.GetMetadata<ControllerActionDescriptor>() != null)
        .Select(re => re.Metadata.GetMetadata<ControllerActionDescriptor>().ControllerTypeInfo)
        .Distinct();

    var controllerNames = string.Join(", ", mappedControllers.Select(t => t.Name));

    Console.WriteLine($"Mapped Controllers: {controllerNames}");
    
}*/