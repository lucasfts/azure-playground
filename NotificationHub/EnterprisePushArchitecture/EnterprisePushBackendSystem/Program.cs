using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading;

namespace EnterprisePushBackendSystem
{
    class Program
    {
        static string sampleTopic, connectionString;

        static void Main(string[] args)
        {
            sampleTopic = "notifications";
            connectionString = CloudConfigurationManager
                .GetSetting("Microsoft.ServiceBus.ConnectionString");

            CreateTopic();
            SendMessage();
        }

        private static void CreateTopic()
        {
            var namespaceManager = NamespaceManager
                .CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists(sampleTopic))
            {
                namespaceManager.CreateTopic(sampleTopic);
            }
        }

        private static void SendMessage()
        {
            var client = TopicClient
                .CreateFromConnectionString(connectionString, sampleTopic);

            var messages = new string[]
            {
                "Employee Id '{0}' has joined.",
                "Employee Id '{0}' has left.",
                "Employee Id '{0}' has switched to a different team."
            };

            while (true)
            {
                var random = new Random();
                var employeeId = random.Next(10000, 99999).ToString();
                var randomMessage = messages[random.Next(0, messages.Length)];
                var notification = string.Format(randomMessage, employeeId);

                var message = new BrokeredMessage(notification);
                client.Send(message);

                Console.WriteLine("{0} Message sent - '{1}'", DateTime.Now, notification);

                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
        }
    }
}
