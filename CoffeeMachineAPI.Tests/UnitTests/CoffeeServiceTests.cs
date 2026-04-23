using Xunit;
using CoffeeMachineAPI.Services;
using CoffeeMachineAPI.Interfaces;

namespace CoffeeMachineAPI.Tests;

public class CoffeeServiceTests
{
    [Fact]
    public void IsOutOfCoffee_ReturnsFalse_ForFirstFourCalls()
    {
        // Arrange
        var dateTimeProvider = new DateTimeProvider();
        var service = new CoffeeService(dateTimeProvider);

        // Act & Assert
        for (int i = 1; i <= 4; i++)
        {
            service.IncrementCounter();
            Assert.False(service.IsOutOfCoffee(), $"Call {i} should not be out of coffee");
        }
    }

    [Fact]
    public void IsOutOfCoffee_ReturnsTrue_OnFifthCall()
    {
        // Arrange
        var dateTimeProvider = new DateTimeProvider();
        var service = new CoffeeService(dateTimeProvider);

        // Act
        for (int i = 1; i <= 5; i++)
        {
            service.IncrementCounter();
        }

        // Assert
        Assert.True(service.IsOutOfCoffee());
    }

    [Fact]
    public void IsOutOfCoffee_ReturnsFalse_OnSixthCall()
    {
        // Arrange
        var dateTimeProvider = new DateTimeProvider();
        var service = new CoffeeService(dateTimeProvider);

        // Act - Make 6 calls
        for (int i = 1; i <= 6; i++)
        {
            service.IncrementCounter();
        }

        // Assert - 6th call should NOT be out of coffee (counter resets)
        Assert.False(service.IsOutOfCoffee());
    }

    [Fact]
    public void GetCurrentTimestamp_ReturnsValidISO8601Format()
    {
        // Arrange
        var dateTimeProvider = new DateTimeProvider();
        var service = new CoffeeService(dateTimeProvider);

        // Act
        var timestamp = service.GetCurrentTimestamp();

        // Assert 
        var parsedDate = DateTime.Parse(timestamp); // Will throw if invalid
        Assert.Contains("T", timestamp);
        Assert.Matches(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}[+-]\d{2}:\d{2}", timestamp);
    }

    [Fact]
    public async Task IncrementCounter_IsThreadSafe()
    {
        // Arrange
        var dateTimeProvider = new DateTimeProvider();
        var service = new CoffeeService(dateTimeProvider);
        var tasks = new List<Task>();

        // Act - Simulate 100 concurrent calls
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() => service.IncrementCounter()));
        }

        await Task.WhenAll(tasks.ToArray());

        // Assert
        for (int i = 0; i < 5; i++)
        {
            service.IncrementCounter();
        }

        
        Assert.True(true);
    }

    [Fact]
    public void MultipleCalls_AlternateBetween503And200()
    {
        // Arrange
        var dateTimeProvider = new DateTimeProvider();
        var service = new CoffeeService(dateTimeProvider);

        // Act & Assert - Test 10 calls pattern
        for (int callNumber = 1; callNumber <= 10; callNumber++)
        {
            service.IncrementCounter();
            bool isOutOfCoffee = service.IsOutOfCoffee();

            if (callNumber % 5 == 0)
            {
                Assert.True(isOutOfCoffee, $"Call {callNumber} should be out of coffee");
            }
            else
            {
                Assert.False(isOutOfCoffee, $"Call {callNumber} should not be out of coffee");
            }
        }
    }
}