using CoffeeMachineAPI.Interfaces;

namespace CoffeeMachineAPI.Tests;

public class MockDateTimeProvider : IDateTimeProvider
{
    private readonly DateTime _fixedDate;

    public MockDateTimeProvider(DateTime fixedDate)
    {
        _fixedDate = fixedDate;
    }

    public DateTime Now => _fixedDate;
}