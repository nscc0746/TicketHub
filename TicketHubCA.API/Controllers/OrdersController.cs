using TicketHubCA.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using Azure.Storage.Queues;
using System.Diagnostics;

namespace TicketHubCA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private IConfiguration _configuration;

        public OrdersController(IConfiguration configuration, ILogger<Order> logger)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            string? connectionString = _configuration["AzureStorageConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                return BadRequest("An error was encountered");
            }

            //Validate that expiration date- Check that its in the future but not too far.
            if (!DateTime.TryParseExact(order.Expiration, "MM/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime expirationDate))
            {
                //This shouldn't happen, because the regex on the model should prevent it, but I've added error checking just in case it does.
                ModelState.AddModelError("Expiration", "Unexpected error with expiration date. Please check your expiration date or contact support.");
            }

            if (expirationDate <= DateTime.Now)
            {
                ModelState.AddModelError("Expiration", "Expiration date must be in the future.");
            } 
            
            if (expirationDate > DateTime.Now.AddYears(5))
            {
                ModelState.AddModelError("Expiration", "Invalid expiration date.");
            }

            if (!ModelState.IsValid)
            {
                //Build a custom error response conforming to the defined standard, that works with our custom model errors.
                var errorResp = new
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "One or more validation errors occured.",
                    Status = 400,
                    Errors = ModelState.Keys.ToDictionary(
                        key => key,
                        key => ModelState[key].Errors.Select(x => x.ErrorMessage).ToArray()
                    ),
                    TraceId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                return BadRequest(errorResp);
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
