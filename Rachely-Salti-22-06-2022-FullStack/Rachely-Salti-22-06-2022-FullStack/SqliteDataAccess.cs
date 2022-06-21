using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SQLite;
using System.Data;
using Dapper;

namespace Rachely_Salti_22_06_2022_FullStack
{
    public class SqliteDataAccess
    {
        private static String LoadConnectionString(string id= "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        //Cities DB Query
        public static List<DBCity> LoadCitiesDb()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var cities = cnn.Query<DBCity>("select * from City", new DynamicParameters());
                return cities.ToList();
            }
        }

        public static Boolean CitiesDbContainsCity(DBCity city)
        {
            var cities = LoadCitiesDb();
            if(cities.Count == 0)
                return false;
            for (int i = 0; i < cities.Count; i++)
                if (cities.ElementAt(i).key == city.key)
                    return true;
            return false;
        }

        public static void AddCityToCities(DBCity city)
        {
            if (!CitiesDbContainsCity(city))
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                     cnn.Execute("insert into City (key, localizedName,temperature,weatherText) values (@key, @localizedName, @temperature ,@weatherText)", city);
                }
            }
        }

        public static void DeleteCityFromCities(DBCity city)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("DELETE FROM City WHERE key like '" + city.key + "'");
            }
        }

        public static DBCity GetCityData(String key)
        {
            DBCity res = null;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var cities = cnn.Query<DBCity>("select * from City WHERE key like '" + key + "'", new DynamicParameters());
                if (cities.Count() != 0)
                    res = cities.First();
            }
            return res;
        }

        public static void DeleteAllDb()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("delete from City ");
                cnn.Execute("delete from Favorite ");
            }
        }

        //Favorite Cities Query
        public static List<String> LoadFavoriteCities()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var favoriteCities = cnn.Query<String>("select * from Favorite", new DynamicParameters());
                return favoriteCities.ToList();
            }
        }

        public static Boolean FavoriteContainsCity(DBCity city)
        {
            var favoriteCities = LoadFavoriteCities();
            return favoriteCities.Count == 0 ? false : favoriteCities.Contains(city.key);
        }

        public static void AddToFavorite(DBCity city)
        {
            if (!FavoriteContainsCity(city))
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
                {
                    cnn.Execute("insert into Favorite (cityKey) values (@key)", city);
                }
            }
        }

        public static void DeleteFavorite(DBCity city)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {               
                cnn.Execute("DELETE FROM Favorite WHERE cityKey like '" + city.key + "'");
            }
        }
    }
}
