using bidService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Caching.Memory;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMemoryCache();

        services.AddHostedService<Worker>();
        services.AddSingleton<IMemoryCache, CustomMemoryCache>();

    })
    .Build();

host.Run();
