using Moq;
using Microsoft.AspNetCore.Mvc;
using CoffeeMachineAPI.Controllers;
using CoffeeMachineAPI.Interfaces;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.Tests;

public class CoffeeControllerTests
{
    [Fact]
    public void BrewCoffee_Returns200_WithCoffeeResponse()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(false);

        var expectedTimestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
        mockService.Setup(s => s.GetCurrentTimestamp()).Returns(expectedTimestamp);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = controller.BrewCoffee();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CoffeeResponse>(okResult.Value);
        Assert.Equal("Your piping hot coffee is ready", response.Message);
        Assert.Equal(expectedTimestamp, response.Prepared);
        mockService.Verify(s => s.IncrementCounter(), Times.Once);
    }

    [Fact]
    public void BrewCoffee_Returns503_OnFifthCall()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);

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
            var result = controller.BrewCoffee();
            Assert.IsType<OkObjectResult>(result);
        }

        // 5th call should be 503
        var fifthResult = controller.BrewCoffee();
        var statusCodeResult = Assert.IsType<StatusCodeResult>(fifthResult);
        Assert.Equal(503, statusCodeResult.StatusCode);
    }

    [Fact]
    public void BrewCoffee_Returns418_OnAprilFirst()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = controller.BrewCoffee();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(418, statusCodeResult.StatusCode);

        // Verify IncrementCounter isn't called on April 1st
        mockService.Verify(s => s.IncrementCounter(), Times.Never);
        mockService.Verify(s => s.IsOutOfCoffee(), Times.Never);
    }

    [Fact]
    public void BrewCoffee_AprilFirst_PrecedesTeapot()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(true);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = controller.BrewCoffee();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(418, statusCodeResult.StatusCode);
    }

    [Fact]
    public void IncrementCounter_IsCalled_OnNormalDays()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(false);

        var controller = new CoffeeController(mockService.Object);

        // Act
        controller.BrewCoffee();

        // Assert
        mockService.Verify(s => s.IncrementCounter(), Times.Once);
    }

    [Fact]
    public void IncrementCounter_IsNotCalled_OnAprilFirst()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        controller.BrewCoffee();

        // Assert
        mockService.Verify(s => s.IncrementCounter(), Times.Never);
    }

    [Fact]
    public void EmptyResponseBody_For503()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(false);
        mockService.Setup(s => s.IsOutOfCoffee()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = controller.BrewCoffee();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(503, statusCodeResult.StatusCode);
        // StatusCodeResult has no body by default
    }

    [Fact]
    public void EmptyResponseBody_For418()
    {
        // Arrange
        var mockService = new Mock<ICoffeeService>();
        mockService.Setup(s => s.IsAprilOne()).Returns(true);

        var controller = new CoffeeController(mockService.Object);

        // Act
        var result = controller.BrewCoffee();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(418, statusCodeResult.StatusCode);
        // StatusCodeResult has no body by default
    }
}