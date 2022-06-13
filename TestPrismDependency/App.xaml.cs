using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Prism.Bridge.MicrosoftDependency;
using Prism.Ioc;
using TestPrismDependency.Data;
using TestPrismDependency.Services;
using TestPrismDependency.Views;

namespace TestPrismDependency
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterBridge(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .Build();

                services.Configure<DefaultSettings>(configuration.GetSection("DefaultSettings"));

                services.AddHttpClient();
            });

            containerRegistry.Register<IWeatherService, OpenMeteoWeatherService>();
        }
    }
}
