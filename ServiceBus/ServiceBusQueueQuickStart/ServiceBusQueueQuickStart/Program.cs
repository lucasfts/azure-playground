using Azure.Messaging.ServiceBus;
using Queue.Config;
using System;
using System.Threading.Tasks;

namespace QueueSender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = ConfigurationHelper.GetServiceBusSettings();

            var client = new ServiceBusClient(settings.ConnectionString);
            var sender = client.CreateSender(settings.QueueName);

            try
            {
                using (var messageBatch = await sender.CreateMessageBatchAsync())
                {
                    for (int i = 1; i <= settings.NumOfMessages; i++)
                    {
                        var serviceBusMessage = new ServiceBusMessage($"Message {i}");
                        if (!messageBatch.TryAddMessage(serviceBusMessage))
                        {
                            throw new Exception($"The message {i} is too large to fit in the batch.");
                        }
                    }

                    await sender.SendMessagesAsync(messageBatch);
                    Console.WriteLine($"A batch of {settings.NumOfMessages} messages has been published to the queue.");
                }
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }

            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }

        
    }
}
