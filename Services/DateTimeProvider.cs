using CoffeeMachineAPI.Interfaces;

namespace CoffeeMachineAPI.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}