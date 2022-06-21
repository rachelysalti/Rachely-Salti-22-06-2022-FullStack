using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rachely_Salti_22_06_2022_FullStack
{
    public class Manager
    {

        public ObservableCollection<City> Cities { get; set; }
        private Weather currentWeather;
        private City selectedCity;
        private DBCity cityDB;
        private String cityName = "";

        public Manager()
        {
            Cities = new ObservableCollection<City>();
        }

        public static void Main(string[] args)
        {
            string myStr = "";
            Manager manager = new Manager();
            while (myStr != "7")
            {
                Console.WriteLine();
                Console.WriteLine("This app shows the weather of some city." +
                    "\n Please enter 1 to get current weather"
                    + "\n Enter 2 to search city"
                   + "\n Enter 3 to add city to favorites"
                   + "\n Enter 4 to delete city from favorites"
                   + "\n Enter 5 print all favorites"
                    + "\n Enter 6 to clear all db"
                   + "\n Enter 7 to Exit");

                myStr = Console.ReadLine();
                switch (myStr)
                {
                    case "1":
                        manager.GetCurrentCityWeather();
                        break;
                    case "2":
                        manager.SearchCity();
                        break;
                    case "3":
                        manager.AddCityToFavorites();
                        break;
                    case "4":
                        manager.DeleteCityFromFavorites();
                        break;
                    case "5":
                        manager.PrintAllFavorites();
                        break;
                    case "6":
                        manager.ClearAllDb();
                        break;
                    case "7":
                        Console.WriteLine("Exit program");
                        break;
                    default:
                        Console.WriteLine("Invalid number");
                        break;
                }
                Console.WriteLine();
            }
        }

        public void GetCurrentCityWeather()
        {
            Console.WriteLine();
            new Action(async () => CityInCurrentGeolocation())();
            Console.WriteLine();
        }
        public void SearchCity()
        {
            Console.WriteLine();

            string name = "";
            string cityIndex = "";
            int i = 0;
            do
            {
                Console.WriteLine("Please enter city prefix.");
                name = Console.ReadLine();
            } while (!CanExecute(name));

            new Action(async () => MakeCitiesQuery(name))();

            Console.WriteLine("Please select city from the list: ");
            foreach (var city in Cities)
            {
                Console.WriteLine(i + "-" + city.LocalizedName);
                i++;
            }
            cityIndex = Console.ReadLine();
            i = Convert.ToInt32(cityIndex);
            City chooseCity = Cities.ElementAt(i);
            Console.WriteLine("You Select: " + chooseCity.LocalizedName);
            CityInCur(chooseCity.LocalizedName);
            Console.WriteLine();
        }

        public void AddCityToFavorites()
        {
            Console.WriteLine();
            CheckIfCityInFavoriteDB(cityName);
            Console.WriteLine();
        }

        public void DeleteCityFromFavorites()
        {
            Console.WriteLine();
            RemoveCityFromFavoriteDB(cityName);
            Console.WriteLine();
        }

        public void ClearAllDb()
        {
            Console.WriteLine("Clear all Db");
            SqliteDataAccess.DeleteAllDb();
        }

        public void PrintAllFavorites()
        {
            List<DBCity> cityList = null;
            List<String> favorite = null;
            cityList = SqliteDataAccess.LoadCitiesDb();
            favorite = SqliteDataAccess.LoadFavoriteCities();
            Console.WriteLine("The Favorites cities:");
            if (cityList == null || cityList.Count == 0|| favorite==null|| favorite.Count==0)
                return;

            foreach (var city in cityList)
            {
                foreach (var cityFavorite in favorite)
                {
                    if (city.key == cityFavorite)
                        Console.WriteLine(city.localizedName + " " + city.temperature);
                }
            }
        }

        public Weather CurrentWeather
        {
            get { return currentWeather; }
            set
            {
                currentWeather = value;
            }
        }

        public City SelectedCity
        {
            get { return selectedCity; }
            set
            {
                selectedCity = value;
                if (selectedCity != null)
                {
                    GetCurrentWeather();
                }
            }
        }

        private async void CityInCurrentGeolocation()
        {
            var currentGeolocation = await LocationApiHandler.Location();
            String name = currentGeolocation.City;
            cityName = name;
            bool cityInDb = CheckIfCityInDB(name);
            if(!cityInDb)
            {
                var city = await AccuWeatherApiHandler.GetCities(currentGeolocation.City);
                SelectedCity = city[0];
                GetCurrentWeather();
                cityDB = new DBCity(SelectedCity.Key, SelectedCity.LocalizedName, CurrentWeather.Temperature.Metric.Value, CurrentWeather.WeatherText);
                SqliteDataAccess.AddCityToCities(cityDB);
                Console.WriteLine("City:" + cityDB.localizedName + " Temperature:" + cityDB.temperature);
            }
        }
        private async void CityInCur(String name)
        {
            cityName = name;
            bool cityInDb = CheckIfCityInDB(name);
            if (!cityInDb)
            {
                var city = await AccuWeatherApiHandler.GetCities(name);
                SelectedCity = city[0];
                GetCurrentWeather();
                cityDB = new DBCity(SelectedCity.Key, SelectedCity.LocalizedName, CurrentWeather.Temperature.Metric.Value, CurrentWeather.WeatherText);
                SqliteDataAccess.AddCityToCities(cityDB);
                Console.WriteLine("City:" + cityDB.localizedName + " Temperature:" + cityDB.temperature);
            }
        }

        public bool CheckIfCityInFavoriteDB(String name)
        {
            List<DBCity> cityList = null;
            cityList = SqliteDataAccess.LoadCitiesDb();
            if (cityList == null || cityList.Count == 0)
                return false;

            foreach (var city in cityList)
                if (city.localizedName == name)
                {
                    Console.WriteLine("Add "+ name + "to favorite");
                    SqliteDataAccess.AddToFavorite(city);
                    return true;
                }
            return false;
        }
        public bool RemoveCityFromFavoriteDB(String name)
        {
            List<DBCity> cityList = null;
            cityList = SqliteDataAccess.LoadCitiesDb();
            if (cityList == null || cityList.Count == 0)
                return false;

            foreach (var city in cityList)
                if (city.localizedName == name)
                {
                    Console.WriteLine("Delete " + name + "to favorite");
                    SqliteDataAccess.DeleteFavorite(city);
                    return true;
                }
            return false;
        }
        public bool CheckIfCityInDB(String cityName)
        {
            List<DBCity> cityList = null;
            cityList = SqliteDataAccess.LoadCitiesDb();
            if (cityList == null || cityList.Count == 0)
                return false;

            foreach (var city in cityList)
                if (city.localizedName == cityName)
                {
                    Console.WriteLine("The city exists in DB");
                    Console.WriteLine("City:"+city.localizedName+ " Temperature:"+city.temperature);
                    return true;
                }

            return false;
        }
 
        private async void GetCurrentWeather()
        {
            CurrentWeather = await AccuWeatherApiHandler.GetCurrentWeather(SelectedCity.Key); 
            Cities.Clear();
        }

        public async void MakeCitiesQuery(String name)
        {
            var cities = await AccuWeatherApiHandler.GetCities(name);
            Cities.Clear();
            foreach (var city in cities)
            {
                Cities.Add(city);
            }
        }
        public bool CanExecute(String name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }
            return true;
        }        
    }
}
