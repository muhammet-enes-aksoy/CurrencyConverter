using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        // Endpoint for converting currency
        [HttpGet("Convert")]
        public async Task<IActionResult> ConvertCurrency(string From, string To, double Amount)
        {
            try
            {
                // Retrieve API key from configuration
                string apiKey = _configuration["SecretKey"];

                // Build the API URL for fetching exchange rates
                string apiUrl = $"https://v6.exchangerate-api.com/v6/{apiKey}/latest/{From}";

                using (var httpClient = new HttpClient())
                {
                    // Make a GET request to the API and retrieve the JSON response
                    var json = await httpClient.GetStringAsync(apiUrl);

                    // Deserialize the JSON response into an object of type API_Obj
                    API_Obj exchangeRates = JsonConvert.DeserializeObject<API_Obj>(json);

                    // Check if the target currency is supported
                    if (!exchangeRates.conversion_rates.ContainsKey(To))
                    {
                        return BadRequest("Target currency not supported.");
                    }

                    // Calculate the converted amount based on the exchange rates
                    double convertedAmount = Amount * exchangeRates.conversion_rates[To];

                    // Return the converted amount in the response
                    return Ok(new { Amount = convertedAmount });
                }
            }
            catch (Exception ex)
            {
                // Log and return a bad request in case of an exception
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(new { Error = $"API Error: {ex.Message}" });
            }
        }
    }

    // Class representing the structure of the API response
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
