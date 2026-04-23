using System.Text.Json.Serialization;

namespace CoffeeMachineAPI.Models
{
    public class CoffeeResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("prepared")]
        public string Prepared { get; set; } = string.Empty;
    }
}