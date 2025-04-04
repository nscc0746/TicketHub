using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TicketHubCA.OrderProcessFunction
{
    public class ProcessOrder
    {
        private readonly ILogger<ProcessOrder> _logger;
        private IConfiguration _configuration;

        public ProcessOrder(ILogger<ProcessOrder> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function(nameof(ProcessOrder))]
        public void Run([QueueTrigger("tickethub", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
