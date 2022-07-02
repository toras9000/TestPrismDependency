using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Options;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TestPrismDependency.Data;
using TestPrismDependency.Services;

namespace TestPrismDependency.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IOptions<DefaultSettings> defaultSettings, IWeatherService weatherService, IWeatherRecorder weatherRecorder, IWeatherRecorderMigrator recorderMigrator)
        {
            this.disposables = new CompositeDisposable();

            var latValue = defaultSettings.Value.Location.Latitude;
            var lonValue = defaultSettings.Value.Location.Longitude;

            this.Latitude = new ReactiveProperty<string>(latValue.ToString())
                .ToReactiveProperty()
                .SetValidateNotifyError(v => double.TryParse(v, out latValue) && !double.IsNaN(latValue) ? null : "error")
                .AddTo(this.disposables);

            this.Longitude = new ReactiveProperty<string>(lonValue.ToString())
                .ToReactiveProperty()
                .SetValidateNotifyError(v => double.TryParse(v, out lonValue) && !double.IsNaN(lonValue) ? null : "error")
                .AddTo(this.disposables);

            this.Weather = new ReactivePropertySlim<Weather?>()
                .AddTo(this.disposables);

            this.WeatherTime = this.Weather
                .Select(w => DateTime.Now)
                .ToReadOnlyReactivePropertySlim()
                .AddTo(this.disposables);

            var weatherContext = new ReactivePropertySlim<bool>(true)
                .AddTo(this.disposables);

            this.GetCommand = new[]
                {
                    this.Latitude.ObserveHasErrors,
                    this.Latitude.ObserveHasErrors,
                }
                .CombineLatestValuesAreAllFalse()
                .ToAsyncReactiveCommand(weatherContext)
                .WithSubscribe(async () => this.Weather.Value = await weatherService.GetWeatherAsync(latValue, lonValue), o => o.AddTo(this.disposables))
                .AddTo(this.disposables);

            this.SaveCommand = this.Weather
                .Select(w => 0 < w?.Forecasts?.Length)
                .ToAsyncReactiveCommand(weatherContext)
                .WithSubscribe(async () => await weatherRecorder.SaveWeatherAsync(this.WeatherTime.Value, this.Weather.Value!), o => o.AddTo(this.disposables))
                .AddTo(this.disposables);

            var initalCommand = Observable.Return(true)
                .ToAsyncReactiveCommand(weatherContext)
                .WithSubscribe(() => recorderMigrator.MigrateAsync(), o => o.AddTo(this.disposables))
                .AddTo(this.disposables);

            initalCommand.Execute();
        }

        public ReactiveProperty<string?> Latitude { get; }
        public ReactiveProperty<string?> Longitude { get; }
        public ReactivePropertySlim<Weather?> Weather { get; }
        public ReadOnlyReactivePropertySlim<DateTime> WeatherTime { get; }
        public AsyncReactiveCommand GetCommand { get; }
        public AsyncReactiveCommand SaveCommand { get; }

        private CompositeDisposable disposables;
    }
}
