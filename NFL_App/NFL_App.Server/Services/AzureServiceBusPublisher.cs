using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NFL_App.Server.Services
{
    public class AzureServiceBusPublisher
    {
        private readonly string connectionString;
        private readonly string queueName;

        public AzureServiceBusPublisher(string serviceBusConnectionString, string serviceBusQueueName)
        {
            connectionString = serviceBusConnectionString;
            queueName = serviceBusQueueName;
        }

        public async Task SendMessageAsync<T>(T message)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender sender = client.CreateSender(queueName);
            string messageBody = JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(messageBody);
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
