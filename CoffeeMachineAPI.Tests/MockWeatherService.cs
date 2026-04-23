using CoffeeMachineAPI.Interfaces;
using Moq;

namespace CoffeeMachineAPI.Tests
{
    // Simple mock for testing
    public class MockWeatherService : IWeatherService
    {
        private readonly double? _temperature;

        public MockWeatherService(double? temperature)
        {
            _temperature = temperature;
        }

        public Task<double?> GetCurrentTemperatureAsync()
        {
            return Task.FromResult(_temperature);
        }
    }

    // Helper to create mock with Moq
    public static class WeatherServiceMockHelper
    {
        public static Mock<IWeatherService> CreateMock(double? temperature)
        {
            var mock = new Mock<IWeatherService>();
            mock.Setup(x => x.GetCurrentTemperatureAsync())
                .ReturnsAsync(temperature);
            return mock;
        }
    }
}