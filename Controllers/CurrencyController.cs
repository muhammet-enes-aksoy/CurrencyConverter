using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyConverter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CurrencyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency(string from, string to, double amount)
        {
            try
            {
                string apiKey = _configuration["SecretKey"];
                string apiUrl = $"https://v6.exchangerate-api.com/v6/{apiKey}/latest/{from}";

                using (var httpClient = new HttpClient())
                {
                    var json = await httpClient.GetStringAsync(apiUrl);
                    API_Obj exchangeRates = JsonConvert.DeserializeObject<API_Obj>(json);

                    if (!exchangeRates.conversion_rates.ContainsKey(to))
                    {
                        return BadRequest("Hedef para birimi desteklenmiyor.");
                    }

                    double convertedAmount = amount * exchangeRates.conversion_rates[to];
                    return Ok(new { amount = convertedAmount });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return BadRequest(new { Error = $"API Hatası: {ex.Message}" });
            }
        }
    }

    public class API_Obj
    {
        public string result { get; set; }
        public string documentation { get; set; }
        public string terms_of_use { get; set; }
        public string time_last_update_unix { get; set; }
        public string time_last_update_utc { get; set; }
        public string time_next_update_unix { get; set; }
        public string time_next_update_utc { get; set; }
        public string base_code { get; set; }
        public Dictionary<string, double> conversion_rates { get; set; }
    }
    
}

