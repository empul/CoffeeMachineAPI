namespace CoffeeMachineAPI.Interfaces
{
    public interface IWeatherService
    {
        Task<double?> GetCurrentTemperatureAsync();
    }
}