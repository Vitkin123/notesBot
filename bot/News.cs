using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace bot
{
    public static class News
    {
        public static async Task<string> Currency()
        {
            var client = new HttpClient();
            var content = await client.GetStringAsync(
                "https://openexchangerates.org/api/latest.json?app_id=a567567dbf3d455fb79c4eb0cb86e305");
            var newObj = JObject.Parse(content);
            var result =
                $"Курс шекеля на сегодня 1 {newObj["base"]} --> {Math.Round((float) (newObj["rates"]["ILS"]), 2)} ILS";
            return result;
        }
    }
}