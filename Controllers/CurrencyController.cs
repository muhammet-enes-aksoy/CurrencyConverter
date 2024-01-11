using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CurrencyConverter.Controllers;
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
    public async Task<IActionResult> ConvertCurrency(string From, string To, double Amount)
    {
        try
        {
            string apiKey = _configuration["SecretKey"];
            string apiUrl = $"https://v6.exchangerate-api.com/v6/{apiKey}/latest/{From}";

            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(apiUrl);
                API_Obj exchangeRates = JsonConvert.DeserializeObject<API_Obj>(json);

                if (!exchangeRates.conversion_rates.ContainsKey(To))
                {
                    return BadRequest("Target currency not supported.");
                }

                double convertedAmount = Amount * exchangeRates.conversion_rates[To];
                return Ok(new { Amount = convertedAmount });
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



