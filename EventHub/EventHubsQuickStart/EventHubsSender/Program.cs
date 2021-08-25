using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventHubsSender
{
    class Program
    {
        static EventHubProducerClient producerClient;

        static async Task Main(string[] args)
        {
            EventHubSettings hubSettings = GetEventHubSettings();

            producerClient = new EventHubProducerClient(hubSettings.ConnectionString, hubSettings.EventHubName);

            using (var eventBatch = await producerClient.CreateBatchAsync())
            {
                for (int i = 1; i <= hubSettings.NumOfEvents; i++)
                {
                    var eventBody = Encoding.UTF8.GetBytes($"Event {1}");
                    var eventData = new EventData(eventBody);

                    if (!eventBatch.TryAdd(eventData))
                        throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }

                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of {hubSettings.NumOfEvents} events has been published.");
            }
        }

        private static EventHubSettings GetEventHubSettings() 
            => GetConfiguration()
                .GetSection(nameof(EventHubSettings))
                .Get<EventHubSettings>();

        private static IConfigurationRoot GetConfiguration() 
            => new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", false)
                            .Build();
    }
}
