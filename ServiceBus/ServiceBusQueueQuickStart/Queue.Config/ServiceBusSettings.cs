namespace Queue.Config
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public int NumOfMessages { get; set; }
    }
}
