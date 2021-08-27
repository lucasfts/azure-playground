namespace Topic.Config
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
        public int NumOfMessages { get; set; }
    }
}
