using CoffeeMachineAPI.Interfaces;

namespace CoffeeMachineAPI.Services
{
    public class CoffeeService : ICoffeeService
    {
        private int _callCounter = 0;
        private readonly object _lock = new object();
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IWeatherService _weatherService;

        // Constructor injection
        public CoffeeService(IDateTimeProvider dateTimeProvider, IWeatherService weatherService)
        {
            _dateTimeProvider = dateTimeProvider;
            _weatherService = weatherService;
        }

        public bool IsAprilOne()
        {
            var today = _dateTimeProvider.Now;  // Now mockable!
            return today.Month == 4 && today.Day == 1;
        }

        public bool IsOutOfCoffee()
        {
            return _callCounter % 5 == 0 && _callCounter > 0;
        }

        public void IncrementCounter()
        {
            lock (_lock)
            {
                _callCounter++;
            }
        }

        public string GetCurrentTimestamp()
        {
            return _dateTimeProvider.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }
        public async Task<string> GetBrewMessageAsync()
        {
            var temperature = await _weatherService.GetCurrentTemperatureAsync();

            if (temperature.HasValue && temperature.Value > 30)
            {
                return "Your refreshing iced coffee is ready";
            }

            return "Your piping hot coffee is ready";
        }
    }
}