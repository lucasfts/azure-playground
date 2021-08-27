using Azure.Messaging.ServiceBus;
using Queue.Config;
using System;
using System.Threading.Tasks;

namespace QueueReceiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var settings = ConfigurationHelper.GetServiceBusSettings();

            var client = new ServiceBusClient(settings.ConnectionString);
            var processor = client.CreateProcessor(settings.QueueName, new ServiceBusProcessorOptions());

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

        private static async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var body = arg.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            await arg.CompleteMessageAsync(arg.Message);
        }

        private static Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
