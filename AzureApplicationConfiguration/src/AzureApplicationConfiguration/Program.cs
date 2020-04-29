using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

namespace TestConsoleApp
{
    internal class Program
    {
        private const string AzureApplicationConfigurationUri = "";

        public static async Task Main()
        {
            #region Read configuration value

            //var configurationBuilder = new ConfigurationBuilder();
            //configurationBuilder.AddAzureAppConfiguration(_ => _.Connect(new Uri(AzureApplicationConfigurationUri), new DefaultAzureCredential()));
            //IConfiguration configuration = configurationBuilder.Build();
            //Console.WriteLine(configuration["TestApp:Settings:WelcomeMessage"]);

            #endregion

            #region Filter configuration value by label 

            //var configurationBuilder = new ConfigurationBuilder();
            //configurationBuilder.AddAzureAppConfiguration(_ =>
            //            {
            //                _.Connect(new Uri(AzureApplicationConfigurationUri), new DefaultAzureCredential());
            //                _.Select("*", "Test");
            //            });
            //IConfiguration configuration = configurationBuilder.Build();
            //Console.WriteLine(configuration["TestApp:Settings:DbConnectionString"]);

            #endregion

            #region Read configuration values from Key Vault

            //var configurationBuilder = new ConfigurationBuilder();
            //configurationBuilder.AddAzureAppConfiguration(_ =>
            //            {
            //                _.Connect(new Uri(AzureApplicationConfigurationUri), new DefaultAzureCredential());
            //                _.ConfigureKeyVault(_ => _.SetCredential(new DefaultAzureCredential()));
            //            });
            //IConfiguration configuration = configurationBuilder.Build();
            //Console.WriteLine(configuration["TestApp:Settings:WelcomeSecretMessage"]);

            #endregion

            #region Dynamic configuration

            //var configurationBuilder = new ConfigurationBuilder();
            //IConfigurationRefresher configurationRefresher = null;
            //configurationBuilder.AddAzureAppConfiguration(_ =>
            //            {
            //                _.Connect(new Uri(AzureApplicationConfigurationUri), new DefaultAzureCredential());
            //                _.ConfigureRefresh(_ =>
            //                    _.Register("TestApp:Settings:Sentinel", true));
            //                _.ConfigureKeyVault(_ => _.SetCredential(new DefaultAzureCredential()));
            //                configurationRefresher = _.GetRefresher();
            //            });
            //IConfiguration configuration = configurationBuilder.Build();
            //Console.WriteLine(configuration["TestApp:Settings:WelcomeMessage"]);
            //Console.WriteLine("Press any key when configuration has been refreshed...");
            //Console.ReadLine();
            //await configurationRefresher.TryRefreshAsync();
            //Console.WriteLine(configuration["TestApp:Settings:WelcomeMessage"]);

            #endregion

            #region Feature Flags

            //var configurationBuilder = new ConfigurationBuilder();
            //configurationBuilder.AddAzureAppConfiguration(_ =>
            //{
            //    _.Connect(new Uri(AzureApplicationConfigurationUri), new DefaultAzureCredential());
            //    _.UseFeatureFlags();
            //});
            //IConfiguration configuration = configurationBuilder.Build();
            //IServiceCollection services = new ServiceCollection();
            //services
            //    .AddSingleton(configuration)
            //    .AddFeatureManagement();
            //ServiceProvider serviceProvider = services.BuildServiceProvider();
            //var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();
            //bool isFeatureAEnabled = await featureManager.IsEnabledAsync("TestApp.Settings.FeatureFlags.FeatureA");
            //Console.WriteLine($"Feature A enabled: {isFeatureAEnabled}");

            #endregion
        }
    }
}