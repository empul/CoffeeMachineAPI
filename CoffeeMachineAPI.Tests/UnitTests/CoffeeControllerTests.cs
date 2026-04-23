using Moq;
using Microsoft.AspNetCore.Mvc;
using CoffeeMachineAPI.Controllers;
using CoffeeMachineAPI.Interfaces;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.Tests;

public class CoffeeControllerTests
{
    [Fact]
    public async Task BrewCoffee_Returns200_WithCoffeeResponse()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(false);
        mockService.Setup(s => s.GetBrewMessageAsync()).ReturnsAsync("Your piping hot coffee is ready");

        var expectedTimestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
        mockService.Setup(s => s.GetCurrentTimestamp()).Returns(expectedTimestamp);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = await controller.BrewCoffee();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoffeeResponse>(okResult.Value);
        Assert.Equal("Your piping hot coffee is ready", response.Message);
        Assert.Equal(expectedTimestamp, response.Prepared);
        mockService.Verify(s => s.IncrementCounter(), Times.Once);
    }

    [Fact]
    public async Task BrewCoffee_Returns503_OnFifthCall()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.GetBrewMessageAsync()).ReturnsAsync("Your piping hot coffee is ready");

        mockService.SetupSequence(s => s.IsOutOfCoffee())
            .Returns(false)  
            .Returns(false)  
            .Returns(false) 
            .Returns(false)  
            .Returns(true); 

        var controller = new CoffeeController(mockService.Object);

        // Act & Assert 
        for (int i = 0; i < 4; i++)
        {
            var result = await controller.BrewCoffee();
            Assert.IsType<OkObjectResult>(result);
        }

        // 5th call should be 503
        var fifthResult = await controller.BrewCoffee();
        var statusCodeResult = Assert.IsType<StatusCodeResult>(fifthResult);
        Assert.Equal(503, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task BrewCoffee_Returns418_OnAprilFirst()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = await controller.BrewCoffee();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(418, statusCodeResult.StatusCode);

        // Verify IncrementCounter isn't called on April 1st
        mockService.Verify(s => s.IncrementCounter(), Times.Never);
        mockService.Verify(s => s.IsOutOfCoffee(), Times.Never);
    }

    [Fact]
    public async Task BrewCoffee_ReturnsIcedCoffee_WhenTemperatureAbove30()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(false);
        mockService.Setup(s => s.GetBrewMessageAsync()).ReturnsAsync("Your refreshing iced coffee is ready");
        mockService.Setup(s => s.GetCurrentTimestamp()).Returns("2024-01-15T10:30:00+09:00");

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = await controller.BrewCoffee();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoffeeResponse>(okResult.Value);
        Assert.Equal("Your refreshing iced coffee is ready", response.Message); ;
    }

    [Fact]
    public async Task IncrementCounter_IsCalled_OnNormalDays()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(false);
        mockService.Setup(s => s.GetBrewMessageAsync()).ReturnsAsync("Your piping hot coffee is ready");

        var controller = new CoffeeController(mockService.Object);

        // Act
        await controller.BrewCoffee();

        // Assert
        mockService.Verify(s => s.IncrementCounter(), Times.Once);
    }

    [Fact]
    public async Task IncrementCounter_IsNotCalled_OnAprilFirst()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        await controller.BrewCoffee();

        // Assert
        mockService.Verify(s => s.IncrementCounter(), Times.Never);
    }

    [Fact]
    public async Task EmptyResponseBody_For503()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = await controller.BrewCoffee();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(503, statusCodeResult.StatusCode);
        // StatusCodeResult has no body by default
    }

    [Fact]
    public async Task EmptyResponseBody_For418()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = await controller.BrewCoffee();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(418, statusCodeResult.StatusCode);
        // StatusCodeResult has no body by default
    }
}