using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
<<<<<<< HEAD
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.Caching;
using bidService.Models;
using Newtonsoft.Json;
using MongoDB.Driver;
=======
>>>>>>> main
namespace bidService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

<<<<<<< HEAD




    





    private readonly IMongoDatabase _database;

    private ConnectionFactory factory = new ConnectionFactory();
    private IConnection connection;
    private IModel channel;
    //Navne på kørere message kan bliver sendt til
    private string[] _routeWays = new string[0];

   
    
    private string auctionBidCol;
   

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
=======
    private ConnectionFactory factory = new ConnectionFactory();
    private IConnection connection;
    private IModel channel;




    //Navne på kørere message kan bliver sendt til
    private string[] _routeWays = new string[0];




    public Worker(ILogger<Worker> logger, IConfiguration configuration,IMe)
>>>>>>> main
    {

        //this.memoryCache = memoryCache;
        _logger = logger;
<<<<<<< HEAD
        
        //Hvilken køer der laves.
        string routeWaysArray = configuration["routeWaysArray"] ?? string.Empty;
        routeWaysArray = "sovs";
=======

        //Hvilken køer der laves.
        string routeWaysArray = configuration["routeWaysArray"] ?? string.Empty;
>>>>>>> main
        //Sætter kørerne i en array der kan køres af et for loop
        _routeWays = routeWaysArray.Length > 0 ? routeWaysArray.Split(",") : new string[0];
        string connectionString = configuration["RabbitMQConnectionString"] ?? string.Empty;

<<<<<<< HEAD
        factory = new ConnectionFactory() { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

=======


        factory = new ConnectionFactory() { HostName = connectionString };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();



>>>>>>> main
        //Logger en besked for at fortælle, hvad kører der er lavet.
        string rWays = string.Empty;
        for (int i = 0; i < _routeWays.Length; i++)
        {
            rWays += _routeWays[i] + ", ";
        }
         _logger.LogInformation($"RouteWays: {rWays}");

<<<<<<< HEAD

        var client = new MongoClient($"mongodb://{configuration["server"] ?? string.Empty}:{configuration["port"] ?? string.Empty}/");
        _database = client.GetDatabase(configuration["database"] ?? string.Empty);
        auctionBidCol = configuration["auctionBidCol"] ?? string.Empty;
=======
        _logger = logger;
>>>>>>> main
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
<<<<<<< HEAD
        
=======
>>>>>>> main
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
<<<<<<< HEAD
        
      var auctionCollection = _database.GetCollection<AuctionBid>(auctionBidCol);

      AuctionBid auctionBid = JsonConvert.DeserializeObject<AuctionBid>(message);
      auctionCollection.InsertOne(auctionBid);
  
=======

        // Logger beskeden og dens routing key
        _logger.LogInformation($"[x] Modtaget '{routingKey}':'{message}' ");

        
>>>>>>> main
    };

    // Begynder at forbruge beskeder fra køen
    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);       

    // Kører en uendelig løkke, der logger en besked og venter 10 sekunder inden den kører igen
    while (!stoppingToken.IsCancellationRequested)
    {
<<<<<<< HEAD
       
        
=======
        _logger.LogInformation("Nyt data: ");
>>>>>>> main
        _logger.LogInformation("Arbejder ved: {time}", DateTimeOffset.Now);
        await Task.Delay(10000, stoppingToken);
    }
    }
}
