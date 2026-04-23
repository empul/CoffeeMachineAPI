namespace CoffeeMachineAPI.Interfaces
{
    public interface ICoffeeService
    {
        bool IsAprilOne();
        bool IsOutOfCoffee();
        void IncrementCounter();
        string GetCurrentTimestamp();
        Task<string> GetBrewMessageAsync();
    }
}