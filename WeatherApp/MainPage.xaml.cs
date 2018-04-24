using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using static WeatherApp.WeatherInfo;
using Windows.Storage.Streams;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string APPID = "542ffd081e67f4512b705f89d2a611b2";
        string cityName = "dublin";

        public MainPage()
        {
            this.InitializeComponent();
            GetWeatherAsync(cityName);
            getForcast(cityName);
        }

        public async void GetWeatherAsync(string cityName)
        {
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}&units=metric&cnt=6", cityName, APPID);

            var http = new HttpClient();
            var respone = await http.GetAsync(url);
            var data = await respone.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<WeatherInfo.Root>(data);
            WeatherInfo.Root outPut = result;

            city.Text = string.Format("{0}", outPut.name);
            CountryName.Text = string.Format("{0}", outPut.sys.country);
            Temp.Text = string.Format("Temperature: {0} \u00B0" + "C", outPut.main.temp);
            windSpeed.Text = string.Format("Speed: {0} km/h", outPut.wind.speed); //weather temperature

            string imageurl = string.Format("http://openweathermap.org/img/w/{0}.png", outPut.weather[0].icon);

            image.Source = new BitmapImage(new Uri(imageurl, UriKind.Absolute));
        }

        DateTime getDate(double millisecond)
        {
            DateTime day = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(millisecond).ToLocalTime();

            return day;
        }


        public async void getForcast(string city)
        {
            int day = 5;
            // api url
            string url = string.Format("http://api.openweathermap.org/data/2.5/forecast/daily?q={0}&units=metric&cnt={1}&APPID={2}", city, day, APPID);

            var http = new HttpClient();
            var respone = await http.GetAsync(url);
            var data = await respone.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<weatherForcast>(data);
            weatherForcast forcast = result;

            //// next day
            Condition.Text = string.Format("{0}: {1}", forcast.list[1].weather[0].main, forcast.list[1].weather[0].description); //weather condition
            Secondday.Text = string.Format("{0}", getDate(forcast.list[1].dt).DayOfWeek); //returning Day
            TempForcast.Text = string.Format("{0} \u00B0" + "C", forcast.list[1].temp.day); //weather temperature
            string imageurl = string.Format("http://openweathermap.org/img/w/{0}.png", forcast.list[1].weather[0].icon);
            imageForcast.Source = new BitmapImage(new Uri(imageurl, UriKind.Absolute));
            windSpeedForcast.Text = string.Format("{0} km/h", forcast.list[1].speed); //weather temperature

            //// day after tomorrow
            Condition2.Text = string.Format("{0}: {1}", forcast.list[2].weather[0].main, forcast.list[2].weather[0].description); //weather condition
            Secondday2.Text = string.Format("{0}", getDate(forcast.list[2].dt).DayOfWeek); //returning Day
            TempForcast2.Text = string.Format("{0} \u00B0" + "C", forcast.list[2].temp.day); //weather temperature
            string imageurl2 = string.Format("http://openweathermap.org/img/w/{0}.png", forcast.list[2].weather[0].icon);
            imageForcast2.Source = new BitmapImage(new Uri(imageurl2, UriKind.Absolute));
            windSpeedForcast2.Text = string.Format("{0} km/h", forcast.list[2].speed); //weather temperature
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(TextcityName.Text))
            {
                GetWeatherAsync("Galway");
                getForcast("Galway");
            }
            else if (string.Equals(TextcityName.Text, "Search City"))
            {    
                GetWeatherAsync("Dublin");
                getForcast("Dublin");
            }
            else
            {
                this.cityName = TextcityName.Text;
                GetWeatherAsync(cityName);
                getForcast(cityName);
                
            }

        }
        //tried to fix save to file but wasnt able to 
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            //Create the text file to hold the 
            using (var fileStream = new FileStream(String.Format("weather.txt"), FileMode.Append))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("City Name: " + city.Text);
                streamWriter.WriteLine("Country Name: " + CountryName.Text);
                streamWriter.WriteLine("Temp: " + Temp.Text); 
            }

        }

        private void TextcityName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }
    }
}
