using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.Caching;
using bidService.Models;
using Newtonsoft.Json;
using MongoDB.Driver;
namespace bidService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMongoDatabase _database;
    private ConnectionFactory factory = new ConnectionFactory();
    private IConnection connection;
    private IModel channel;
    private string auctionBidCol;
   
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {

        //this.memoryCache = memoryCache;
        _logger = logger;

        string connectionString = configuration["rabbitmqUrl"] ?? "localhost";
        _logger.LogInformation("Connecter til rabbitmq: " + connectionString + ":" + configuration["rabbitmqPort"]);        

        factory = new ConnectionFactory() { HostName = connectionString, Port= Convert.ToInt16(configuration["rabbitmqPort"]) };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        //Logger en besked for at fortælle, hvad kører der er lavet.
        var client = new MongoClient($"{configuration["connectMongodb"]}");
        _database = client.GetDatabase(configuration["database"] ?? string.Empty);
        auctionBidCol = configuration["auctionBidCol"] ?? string.Empty;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
        // Deklarer et kønavn og få navnet fra RabbitMQ-serveren
    var queueName = channel.QueueDeclare().QueueName;
 
        channel.QueueBind(queue: queueName,
                          exchange: "topic_logs",
                          routingKey: "auction");
    // Opretter en forbruger, som lytter til beskeder på køen
    var consumer = new EventingBasicConsumer(channel);

    // Når forbrugeren modtager en besked, vil denne handling blive udført
    consumer.Received += (model, ea) =>
    {
        // Konverterer beskedens krop fra bytes til en UTF-8-streng
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        _logger.LogInformation(message);
        // Gemmer routing key'en fra beskeden
        var routingKey = ea.RoutingKey;
        
      var auctionCollection = _database.GetCollection<Bid>(auctionBidCol);

      Bid bid = JsonConvert.DeserializeObject<Bid>(message);
      auctionCollection.InsertOne(bid);
    
    };

    // Begynder at forbruge beskeder fra køen
    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);       

    // Kører en uendelig løkke, der logger en besked og venter 10 sekunder inden den kører igen
    while (!stoppingToken.IsCancellationRequested)
    {
       
        
        _logger.LogInformation("Arbejder tid: {time}", DateTimeOffset.Now);
        await Task.Delay(10000, stoppingToken);
    }
    }
}
