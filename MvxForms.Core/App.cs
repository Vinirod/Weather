using MvvmCross.Forms.Platform;
using MvxForms.Core.ViewModels;
using MvxForms.Core.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MvxForms.Core
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class App : MvxFormsApplication
    {
        public App()
        {
            var tabs = new TabbedPage
            {
                Title = "Clima",
                BindingContext = new WeatherViewModel(),
                Children =
                {
                    new WeatherView(),
                    new WeatherForecastView()
                }
            };
            
            Application.Current.MainPage = new NavigationPage(tabs)
            {
                BarBackgroundColor = Color.FromHex("3498db"),
                BarTextColor = Color.White
            };
        }
    }
}