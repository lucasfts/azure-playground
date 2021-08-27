using Microsoft.Extensions.Configuration;

namespace Topic.Config
{
    public static class ConfigurationHelper
    {
        public static ServiceBusSettings GetServiceBusSettings()
        {
            var configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

            var settings = configuration
                .GetSection(nameof(ServiceBusSettings))
                .Get<ServiceBusSettings>();
            return settings;
        }
    }
}
