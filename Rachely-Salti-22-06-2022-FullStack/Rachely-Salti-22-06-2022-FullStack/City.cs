using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rachely_Salti_22_06_2022_FullStack
{
    public class Area
    {
        public string ID { get; set; }
        public string LocalizedName { get; set; }
    }
    public class DBCity
    {
        public String key { get; set; }
        public String localizedName { get; set; }
        public double temperature { get; set; }
        public String weatherText { get; set; }

        public DBCity(String key, String localizedName, double temperature, String weatherText)
        {
            this.key = key;
            this.localizedName = localizedName;
            this.temperature = temperature;
            this.weatherText = weatherText;
        }
    }

    public class City
    {
        public int Version { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public int Rank { get; set; }
        public string LocalizedName { get; set; }
        public Area Country { get; set; }
        public Area AdministrativeArea { get; set; }

        public void PrintCity()
        {
            Console.WriteLine("City name:" + LocalizedName);
        }
    }
}
