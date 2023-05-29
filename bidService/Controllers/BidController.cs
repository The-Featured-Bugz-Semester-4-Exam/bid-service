using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using bidService.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace bidService.Controllers;


[ApiController]
[Route("api")]
public class BidController : ControllerBase
{

    private readonly ILogger<BidController> _logger;

    private ConnectionFactory factory = new ConnectionFactory();
    private IConnection connection;
    private IModel channel;



    public BidController(ILogger<BidController> logger, IConfiguration configuration)
    {


        _logger = logger;

        var hostName = System.Net.Dns.GetHostName();
        var ips = System.Net.Dns.GetHostAddresses(hostName);
        var _ipaddr = ips.First().MapToIPv4().ToString();
        _logger.LogInformation(1, $"Taxabooking responding from {_ipaddr}");

        var factory = new ConnectionFactory { HostName = configuration["workerUrl"] ?? "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();


    }  

    [HttpGet()]
    public Bid GetItem(){
        Bid ww = new Bid();
        
        return ww;
    }
}