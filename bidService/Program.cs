using bidService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Core;

using NLog;
using NLog.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("BidService-Worker");

try {
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMemoryCache();
        services.AddHostedService<Worker>();
        services.AddControllers();
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
            });
        });
        webBuilder.UseUrls("http://localhost:5034");
   })/* .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
    })*/
    .UseNLog()
    .Build();

host.Run();
}
catch (Exception ex)
{
logger.Error(ex, "Stopped program because of exception");
throw;
}
finally
{
NLog.LogManager.Shutdown();
}