using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

                services.AddDbContextFactory<WeatherDbContext>(builder => builder.UseSqlite(configuration.GetConnectionString("TestDatabase")));

            });

            containerRegistry.Register<IWeatherService, OpenMeteoWeatherService>();
            containerRegistry.Register<IWeatherRecorder, EfCoreWeatherRecorder>();
            containerRegistry.Register<IWeatherRecorderMigrator, EfCoreWeatherRecorderMigrator>();
        }
    }
}
