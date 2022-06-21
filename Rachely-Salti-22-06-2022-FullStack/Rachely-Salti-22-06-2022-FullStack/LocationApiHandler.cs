using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Rachely_Salti_22_06_2022_FullStack
{
    public class LocationApiHandler
    {
        private const string IP_API_URL = "http://ip-api.com/json/";

        public static async Task<Location> Location()
        {
            Location geolocation = new Location();
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(IP_API_URL).Result;

            if (response.IsSuccessStatusCode)
            {
                string page = response.Content.ReadAsStringAsync().Result;
                geolocation = JsonConvert.DeserializeObject<Location>(page);
            }
            return geolocation;
        }
    }
}
