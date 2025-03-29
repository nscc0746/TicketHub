using TicketHubCA.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using Azure.Storage.Queues;

namespace TicketHubCA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IConfiguration _configuration;
        private ILogger<Order> _logger;

        public OrdersController(IConfiguration configuration, ILogger<Order> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetOrder()
        {
            return Ok("The API is up!");
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            string? connectionString = _configuration["AzureStorageConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("No connection string!");
                return BadRequest("An error was encountered");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Validate that expiration date- Check that its in the future but not too far.
            if (!DateTime.TryParseExact(order.Expiration, "MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime expirationDate))
            {
                //This shouldn't happen, because the regex on the model should prevent it, but I've added error checking just in case it does.
                _logger.LogError($"An error occured while parsing an expiration date. {order.Expiration}");

                ModelState.AddModelError("Expiration", "Unexpected error with expiration date. Please check your expiration date or contact support.");
                return BadRequest(ModelState);
            }

            if (expirationDate <= DateTime.Now)
            {
                ModelState.AddModelError("Expiration", "Expiration date must be in the future.");
                return BadRequest(ModelState);
            } 
            
            if (expirationDate > DateTime.Now.AddYears(5))
            {
                ModelState.AddModelError("Expiration", "Invalid expiration date.");
                return BadRequest(ModelState);
            }

            string queueName = "tickethub";
            QueueClient queueClient = new QueueClient(connectionString, queueName);

            // serialize an object to json
            string message = JsonSerializer.Serialize(order);

            // send string message to queue
            await queueClient.SendMessageAsync(message);

            return Ok();
        }
    }
}
