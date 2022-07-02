using System.Collections.ObjectModel;
using System.Reactive.Disposables;
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

        public MainWindowViewModel(IOptions<DefaultSettings> defaultSettings, IWeatherService weatherService)
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

            this.GetCommand = new[]
                {
                    this.Latitude.ObserveHasErrors,
                    this.Latitude.ObserveHasErrors,
                }
                .CombineLatestValuesAreAllFalse()
                .ToAsyncReactiveCommand()
                .WithSubscribe(async () => this.Weather.Value = await weatherService.GetWeatherAsync(latValue, lonValue), o => o.AddTo(this.disposables))
                .AddTo(this.disposables);

        }

        public ReactiveProperty<string?> Latitude { get; }
        public ReactiveProperty<string?> Longitude { get; }
        public ReactivePropertySlim<Weather?> Weather { get; }
        public AsyncReactiveCommand GetCommand { get; }

        private CompositeDisposable disposables;
    }
}
