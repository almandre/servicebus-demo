using Microsoft.Extensions.Configuration;

namespace ServiceBusDemo.Configuration
{
    public static class ConfigurationHelper
    {
        public static AppSettings LoadSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var appSettings = new AppSettings();
            configuration.Bind(appSettings);

            return appSettings;
        }
    }
}
