using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.Net.Http;


namespace Rachely_Salti_22_06_2022_FullStack
{
    public class AccuWeatherApiHandler
    {
        private const string ACCU_WEATHER_URL = "http://dataservice.accuweather.com/";
        private const string CITY_AUTOCOMPLETE_ENDPOINT = "locations/v1/cities/autocomplete?apikey={0}&q={1}";
        private const string CURRENT_WEATHER_ENDPOINT = "currentconditions/v1/{0}?apikey={1}";
        private const string API_KEY = "BrtD5AknOu8JsTCFdTIlppFmnJcfujxV";

        public static async Task<List<City>> GetCities(string query)
        {
            List<City> cities = new List<City>();
            string url = ACCU_WEATHER_URL + string.Format(CITY_AUTOCOMPLETE_ENDPOINT, API_KEY, query);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string page = response.Content.ReadAsStringAsync().Result;
                cities = JsonConvert.DeserializeObject<List<City>>(page);
            }
            return cities;
        }

        public static async Task<Weather> GetCurrentWeather(string cityKey)
        {
            Weather currentWeather = new Weather();
            string url = ACCU_WEATHER_URL + string.Format(CURRENT_WEATHER_ENDPOINT, cityKey, API_KEY);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                currentWeather = (JsonConvert.DeserializeObject<List<Weather>>(json)).FirstOrDefault();

            }
            return currentWeather;
        }
    }
}
