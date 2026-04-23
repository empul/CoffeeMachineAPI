using System.Text.Json.Serialization;

namespace CoffeeMachineAPI.Models
{
    public class WeatherResponse
    {
        [JsonPropertyName("main")]
        public MainWeatherData Main { get; set; } = new();

        [JsonPropertyName("name")]
        public string? CityName { get; set; }
    }

    public class MainWeatherData
    {
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; }
    }
}