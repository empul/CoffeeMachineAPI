using System.Text.Json;
using CoffeeMachineAPI.Interfaces;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<double?> GetCurrentTemperatureAsync()
        {
            try
            {
                var apiKey = _configuration["WeatherApi:ApiKey"];
                var city = _configuration["WeatherApi:City"];
                var units = _configuration["WeatherApi:Units"];
                var baseUrl = _configuration["WeatherApi:BaseUrl"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("Weather API key not configured");
                    return null;
                }

                // Use city name (more readable than coordinates)
                var url = $"{baseUrl}?q={city}&appid={apiKey}&units={units}";

                _logger.LogInformation("Calling weather API for city: {City}", city);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Weather API returned {StatusCode} for city {City}",
                        response.StatusCode, city);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var temperature = weatherData?.Main?.Temperature;

                _logger.LogInformation("Current temperature in {City}: {Temperature}°C",
                    city, temperature);

                return temperature;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get weather data");
                return null;
            }
        }
    }
}