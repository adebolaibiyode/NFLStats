using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace NFL_App.Server.MessageQueue
{
    public class ServiceBusManager
    {
        private string connectionString = "Azure Service Bus connection string here";
        private string queueName = "teamstatsqueue";

        public async Task SendMessageAsync(object messageObj)
        {
            await using (var client = new ServiceBusClient(connectionString))
            {
                var sender = client.CreateSender(queueName);
                var messageJson = JsonConvert.SerializeObject(messageObj);
                var message = new ServiceBusMessage(messageJson);
                await sender.SendMessageAsync(message);
            }
        }
    }

}
