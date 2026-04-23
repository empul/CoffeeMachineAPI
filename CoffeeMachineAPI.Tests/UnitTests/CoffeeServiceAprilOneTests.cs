using CoffeeMachineAPI.Interfaces;
using CoffeeMachineAPI.Services;
using Moq;
using Xunit;

namespace CoffeeMachineAPI.Tests;

public class CoffeeServiceAprilOneTests
{
    private readonly Mock<IWeatherService> _mockWeather;

    public CoffeeServiceAprilOneTests()
    {
        _mockWeather = new Mock<IWeatherService>();
    }

    private CoffeeService CreateService(IDateTimeProvider dateTimeProvider)
    {
        return new CoffeeService(dateTimeProvider, _mockWeather.Object);
    }

    [Fact]
    public void IsAprilOne_ReturnsTrue_OnAprilFirst()
    {
        // Arrange - Mock April 1st
        var mockDate = new MockDateTimeProvider(new DateTime(2024, 4, 1));
        var service = CreateService(mockDate);

        // Act
        var result = service.IsAprilOne();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsAprilOne_ReturnsFalse_OnOtherDays()
    {
        // Arrange - Mock June 15th (not April 1st)
        var mockDate = new MockDateTimeProvider(new DateTime(2024, 6, 15));
        var service = CreateService(mockDate);

        // Act
        var result = service.IsAprilOne();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetCurrentTimestamp_ReturnsMockedDate()
    {
        // Arrange - Mock a specific date
        var expectedDate = new DateTime(2024, 12, 25, 14, 30, 0);
        var mockDate = new MockDateTimeProvider(expectedDate);
        var service = CreateService(mockDate);

        // Act
        var timestamp = service.GetCurrentTimestamp();
        var parsedDate = DateTime.Parse(timestamp);

        // Assert
        Assert.Equal(expectedDate.Date, parsedDate.Date);
        Assert.Equal(expectedDate.Hour, parsedDate.Hour);
        Assert.Equal(expectedDate.Minute, parsedDate.Minute);
    }
}