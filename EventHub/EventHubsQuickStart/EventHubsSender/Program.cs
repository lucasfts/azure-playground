using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EventHubsCore;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventHubsSender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hubSettings = ConfigurationHelper.GetEventHubSettings();

            var producerClient = new EventHubProducerClient(hubSettings.ConnectionString, hubSettings.EventHubName);

            using (var eventBatch = await producerClient.CreateBatchAsync())
            {
                for (int i = 1; i <= hubSettings.NumOfEvents; i++)
                {
                    var eventBody = Encoding.UTF8.GetBytes($"Event {i}");
                    var eventData = new EventData(eventBody);

                    if (!eventBatch.TryAdd(eventData))
                        throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }

                await producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of {hubSettings.NumOfEvents} events has been published.");
            }
        }
    }
}
