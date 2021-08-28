using Microsoft.Azure;
using Microsoft.Azure.NotificationHubs;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiveAndSendNotification
{
    class Program
    {
        static string sampleTopic, sampleSubscription, connectionString;
        static NotificationHubClient hub;

        static void Main(string[] args)
        {
            connectionString = CloudConfigurationManager
                .GetSetting("Microsoft.ServiceBus.ConnectionString");

            CreateSubscription(connectionString);

            ReceiveMessageAndSendNotification(connectionString);
        }

        private static void CreateSubscription(string connectionString)
        {
            sampleTopic = "notifications";
            sampleSubscription = "webjobsubscription";
            var namespaceManager = Microsoft.ServiceBus.NamespaceManager
                .CreateFromConnectionString(connectionString);

            if (!namespaceManager.SubscriptionExists(sampleTopic, sampleSubscription))
            {
                namespaceManager.CreateSubscription(sampleTopic, sampleSubscription);
            }
        }

        private static void ReceiveMessageAndSendNotification(string connectionString)
        {
            var hubConnectionString = CloudConfigurationManager
                .GetSetting("Microsoft.NotificationHub.ConnectionString");

            hub = NotificationHubClient
                .CreateClientFromConnectionString(hubConnectionString, "enterprisepushservicehub");

            var client = SubscriptionClient
                .CreateFromConnectionString(connectionString, sampleTopic, sampleSubscription);

            client.Receive();

            while (true)
            {
                var message = client.Receive();
                var toastMessage = @"<toast><visual><binding template=""ToastText01""><text id=""1"">{messagepayload}</text></binding></visual></toast>";

                if (message != null)
                {
                    try
                    {
                        Console.WriteLine(message.MessageId);
                        Console.WriteLine(message.SequenceNumber);
                        var messageBody = message.GetBody<string>();
                        Console.WriteLine("Body: " + messageBody + "\n");

                        toastMessage = toastMessage.Replace("{messagepayload}", messageBody);
                        SendNotificationAsync(toastMessage);

                        message.Complete();
                    }
                    catch (Exception)
                    {
                        message.Abandon();
                    }
                }
            }
        }

        private static async void SendNotificationAsync(string toastMessage)
        {
            await hub.SendWindowsNativeNotificationAsync(toastMessage);
        }
    }
}
