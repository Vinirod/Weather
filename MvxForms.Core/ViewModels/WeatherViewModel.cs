using MvvmCross.Core.ViewModels;
using MvxForms.Core.Helpers;
using MvxForms.Core.Model;
using MvxForms.Core.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Geolocator;
using Plugin.TextToSpeech;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace MvxForms.Core.ViewModels
{
    public class WeatherViewModel : MvxViewModel
    {

        WeatherService WeatherService { get; } = new WeatherService();

        private bool isImperial = Settings.IsImperial;
        private string location = Settings.City;
        private bool useGPS;
        private string temp = string.Empty;
        private string condition = string.Empty;
        private WeatherForecastRoot forecast;
        private IMvxCommand getWeather;


        public WeatherViewModel()
        {

        }

        public override Task Initialize()
        {
            //TODO: Add starting logic here

            return base.Initialize();
        }
        
        public string Location
        {
            get { return location; }
            set
            {
                SetProperty(ref location, value);
                Settings.City = value;
            }
        }

        public bool UseGPS
        {
            get { return useGPS; }
            set
            {
                SetProperty(ref useGPS, value);
            }
        }

        public bool IsImperial
        {
            get { return isImperial; }
            set
            {
                SetProperty(ref isImperial, value);
                Settings.IsImperial = value;
            }
        }

        public string Temp
        {
            get { return temp; }
            set { SetProperty(ref temp, value); }
        }

        public string Condition
        {
            get { return condition; }
            set { SetProperty(ref condition, value); ; }
        }

        public WeatherForecastRoot Forecast
        {
            get { return forecast; }
            set { forecast = value; RaisePropertyChanged(); }
        }

        public IMvxCommand GetWeatherCommand =>
                getWeather ?? (getWeather = new MvxCommand(async () => await ExecuteGetWeatherCommand()));

        private async Task ExecuteGetWeatherCommand()
        {
            if (App.Current.MainPage.IsBusy)
                return;

            App.Current.MainPage.IsBusy = true;
            try
            {
                WeatherRoot weatherRoot = null;
                var units = IsImperial ? Units.Imperial : Units.Metric;


                if (UseGPS)
                {
                    var hasPermission = await CheckPermissions();
                    if (!hasPermission)
                        return;

                    var gps = await CrossGeolocator.Current.GetPositionAsync(TimeSpan.FromSeconds(10));
                    weatherRoot = await WeatherService.GetWeather(gps.Latitude, gps.Longitude, units);
                }
                else
                {
                    //Get weather by city
                    weatherRoot = await WeatherService.GetWeather(Location.Trim(), units);
                }


                //Get forecast based on cityId
                Forecast = await WeatherService.GetForecast(weatherRoot.CityId, units);

                var unit = IsImperial ? "F" : "C";
                Temp = $"Temp: {weatherRoot?.MainWeather?.Temperature ?? 0}°{unit}";
                Condition = $"{weatherRoot.Name}: {weatherRoot?.Weather?[0]?.Description ?? string.Empty}";

                
            }
            catch (Exception ex)
            {
                Temp = "Não é possível obter o tempo"+ ex;
            }
            finally
            {
                App.Current.MainPage.IsBusy = false;
            }
        }

        async Task<bool> CheckPermissions()
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            bool request = false;
            if (permissionStatus == PermissionStatus.Denied)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {

                    var title = "Permissão de localização";
                    var question = "Para obter sua cidade atual, a permissão de localização é necessária. Entre em Configurações e ative o Local para o aplicativo.";
                    var positive = "Configurações";
                    var negative = "Talvez mais tarde";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    if (task == null)
                        return false;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }

                    return false;
                }

                request = true;
            }

            if (request || permissionStatus != PermissionStatus.Granted)
            {
                var newStatus = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                if (newStatus.ContainsKey(Permission.Location) && newStatus[Permission.Location] != PermissionStatus.Granted)
                {
                    var title = "Permissão de Localização";
                    var question = "Para obter sua cidade atual, a permissão de localização é necessária.";
                    var positive = "Configurações";
                    var negative = "Talvez mais tarde";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    if (task == null)
                        return false;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }
                    return false;
                }
            }
            return true;
        }
    }
}
