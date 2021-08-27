using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;
using Topic.Config;

namespace SubscriptionReceiver
{
    class Program
    {
        static ServiceBusSettings settings;

        static async Task Main(string[] args)
        {
            settings = ConfigurationHelper.GetServiceBusSettings();

            var client = new ServiceBusClient(settings.ConnectionString);
            var processor = client.CreateProcessor(settings.TopicName, settings.SubscriptionName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
                processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private static async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body} from subscription: {settings.SubscriptionName}");

            await args.CompleteMessageAsync(args.Message);
        }

        private static Task Processor_ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
