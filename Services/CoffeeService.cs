using CoffeeMachineAPI.Interfaces;

namespace CoffeeMachineAPI.Services
{
    public class CoffeeService : ICoffeeService
    {
        private int _callCounter = 0;
        private readonly object _lock = new object();
        private readonly IDateTimeProvider _dateTimeProvider;

        // Constructor injection
        public CoffeeService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public bool IsAprilOne()
        {
            var today = _dateTimeProvider.Now;
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
    }
}