using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using EventHubsCore;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventHubsReceiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hubSettings = ConfigurationHelper.GetEventHubSettings();
            var storageSettings = ConfigurationHelper.GetEventStorageSettings();

            var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            var storageClient = new BlobContainerClient(storageSettings.ConnectionString, storageSettings.ContainerName);
            var processor = new EventProcessorClient(storageClient, consumerGroup, hubSettings.ConnectionString, hubSettings.EventHubName);

            processor.ProcessEventAsync += Processor_ProcessEventAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            await processor.StartProcessingAsync();

            await Task.Delay(TimeSpan.FromSeconds(30));

            await processor.StopProcessingAsync();
        }

        private static async Task Processor_ProcessEventAsync(ProcessEventArgs arg)
        {
            var bodyBytes = arg.Data.EventBody.ToArray();
            var body = Encoding.UTF8.GetString(bodyBytes);

            Console.WriteLine("\tReceived event: {0}", body);

            await arg.UpdateCheckpointAsync(arg.CancellationToken);
        }

        private static Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            Console.WriteLine($"\tPartition '{ arg.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(arg.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
