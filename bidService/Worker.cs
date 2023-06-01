using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using bidService.Models;
using Newtonsoft.Json;
using MongoDB.Driver;
namespace bidService;

// Define the Worker class that inherits from BackgroundService
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
        _logger = logger;


        // Get the RabbitMQ connection information from the configuration
        string connectionString = configuration["rabbitmqUrl"] ?? "localhost";
        _logger.LogInformation("Connecter to rabbitmq: " + connectionString + ":" + configuration["rabbitmqPort"]);


        // Create RabbitMQ connection factory and connection
        factory = new ConnectionFactory() { HostName = connectionString, Port = Convert.ToInt16(configuration["rabbitmqPort"]) };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();


        // Create MongoDB database connection using configuration
        var client = new MongoClient($"{configuration["connectMongodb"]}");
        _database = client.GetDatabase(configuration["database"] ?? string.Empty);
        auctionBidCol = configuration["auctionBidCol"] ?? string.Empty;
    }

  
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Declare a queue for the channel
        var queueName = channel.QueueDeclare().QueueName;

        // Bind the queue to the topic exchange with routing key "auction"
        channel.QueueBind(queue: queueName,
                          exchange: "topic_logs",
                          routingKey: "auction");

        // Create a consumer for the channel
        var consumer = new EventingBasicConsumer(channel);

        // Set up event handler for received messages
        consumer.Received += (model, ea) =>
        {
            // Process the received message
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation(message);
            var routingKey = ea.RoutingKey;

            // Get the auction collection from the MongoDB database
            var auctionCollection = _database.GetCollection<Bid>(auctionBidCol);

            // Deserialize the message into a Bid object and insert it into the auction collection
            Bid bid = JsonConvert.DeserializeObject<Bid>(message);
            auctionCollection.InsertOne(bid);
        };

        // Start consuming messages from the queue
        channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);

        // Continuously perform work until cancellation is requested
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Working time: {time}", DateTimeOffset.Now);

            // Delay the execution for 10 seconds
            await Task.Delay(10000, stoppingToken);
        }
    }
}
