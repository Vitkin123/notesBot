using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace bot
{
    public class Weather
    {
        public static async Task<string> WeatherSender()
        {
            var client = new HttpClient();
            var content = await client.GetStringAsync(
                "http://api.openweathermap.org/data/2.5/weather?q=Tel-Aviv&appid=a0ebb274ce98053059e02b2b25a299aa");
            var newObj = JObject.Parse(content);
            var result =
                $"Погода в городе {newObj.Property("name").Value} {(int) newObj.Property("main").Value["temp"] - 273}℃";
            return result;
        }
    }
}