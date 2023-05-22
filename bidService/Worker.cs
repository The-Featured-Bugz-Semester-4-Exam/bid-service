using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.Caching;
using bidService.Models;
using Newtonsoft.Json;
namespace bidService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private ConnectionFactory factory = new ConnectionFactory();
    private IConnection connection;
    private IModel channel;

  

    //Navne på kørere message kan bliver sendt til
    private string[] _routeWays = new string[0];

   
    

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {

        //this.memoryCache = memoryCache;
        _logger = logger;
        
        //Hvilken køer der laves.
        string routeWaysArray = configuration["routeWaysArray"] ?? string.Empty;
        routeWaysArray = "sovs";
        //Sætter kørerne i en array der kan køres af et for loop
        _routeWays = routeWaysArray.Length > 0 ? routeWaysArray.Split(",") : new string[0];
        string connectionString = configuration["RabbitMQConnectionString"] ?? string.Empty;

    

        factory = new ConnectionFactory() { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

            

        //Logger en besked for at fortælle, hvad kører der er lavet.
        string rWays = string.Empty;
        for (int i = 0; i < _routeWays.Length; i++)
        {
            rWays += _routeWays[i] + ", ";
        }
         _logger.LogInformation($"RouteWays: {rWays}");

        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        // Deklarer et kønavn og få navnet fra RabbitMQ-serveren
    var queueName = channel.QueueDeclare().QueueName;

    // Gennemløber listen af routing keys og binder køen til exchange
    foreach (var bindingKey in _routeWays)
    {
        channel.QueueBind(queue: queueName,
                          exchange: "topic_logs",
                          routingKey: bindingKey);
    }

    // Logger antallet af routing keys
    _logger.LogInformation(_routeWays.Length.ToString());

    // Opretter en forbruger, som lytter til beskeder på køen
    var consumer = new EventingBasicConsumer(channel);

    // Når forbrugeren modtager en besked, vil denne handling blive udført
    consumer.Received += (model, ea) =>
    {
        // Konverterer beskedens krop fra bytes til en UTF-8-streng
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        // Gemmer routing key'en fra beskeden
        var routingKey = ea.RoutingKey;

        // Logger beskeden og dens routing key
        _logger.LogInformation($"[x] Modtaget '{routingKey}':'{message}' ");
        _logger.LogInformation("All items length: hello1");
        AuctionBid auctionBid = JsonConvert.DeserializeObject<AuctionBid>(message);
        _logger.LogInformation("All items length: hello2");
        CacheItem item = new CacheItem(auctionBid.AuctionBidID.ToString(),auctionBid);
        _logger.LogInformation("All items length: hello3");
        CustomMemoryCache.CustomMemoryCach.MemoryCache.Add(item,new CacheItemPolicy());

   
        _logger.LogInformation("All items length: hello4" + (CustomMemoryCache.CustomMemoryCach == null));
       IEnumerable<string> keys = CustomMemoryCache.CustomMemoryCach.MemoryCache.Select(item => item.Key);
        _logger.LogInformation("All items length: hello5");
AuctionBid[] allItems = keys
    .Where(key => key == "1")
    .Select(key => (AuctionBid)CustomMemoryCache.CustomMemoryCach.MemoryCache.Get(key))
    .ToArray();



    _logger.LogInformation("All items length: " +allItems.Length.ToString());

        for (int i = 0; i < allItems.Length; i++)
        {
            
            _logger.LogInformation("bid value: " + allItems[i].Bid);
            //Send with RabbitMQ..
        }
    };

    // Begynder at forbruge beskeder fra køen
    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);       

    // Kører en uendelig løkke, der logger en besked og venter 10 sekunder inden den kører igen
    while (!stoppingToken.IsCancellationRequested)
    {
       
        
        _logger.LogInformation("Arbejder ved: {time}", DateTimeOffset.Now);
        await Task.Delay(10000, stoppingToken);
    }
    }
}
