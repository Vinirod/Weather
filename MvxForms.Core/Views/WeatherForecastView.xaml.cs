using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MvxForms.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherForecastView : ContentPage
    {
        public WeatherForecastView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
                Icon = new FileImageSource { File = "tab2.png" };

            ListViewWeather.ItemTapped += (sender, args) => ListViewWeather.SelectedItem = null;
        }
    }
}