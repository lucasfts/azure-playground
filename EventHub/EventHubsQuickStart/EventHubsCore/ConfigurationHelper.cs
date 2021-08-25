using Microsoft.Extensions.Configuration;

namespace EventHubsCore
{
    public class ConfigurationHelper
    {
        public static StorageSettings GetEventStorageSettings()
          => GetConfiguration()
              .GetSection(nameof(StorageSettings))
              .Get<StorageSettings>();

        public static EventHubSettings GetEventHubSettings()
           => GetConfiguration()
               .GetSection(nameof(EventHubSettings))
               .Get<EventHubSettings>();

        private static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", false)
                            .Build();
    }
}
