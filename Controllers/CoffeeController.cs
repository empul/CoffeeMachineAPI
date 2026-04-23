using Microsoft.AspNetCore.Mvc;
using CoffeeMachineAPI.Interfaces;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeeController(ICoffeeService coffeeService)
        {
            _coffeeService = coffeeService;
        }

        [HttpGet("brew-coffee")]
        public IActionResult BrewCoffee()
        {
            // Requirement 3: April 1st returns 418 I'm a teapot
            if (_coffeeService.IsAprilOne())
            {
                return StatusCode(418);
            }

            // Increment call counter
            _coffeeService.IncrementCounter();

            // Requirement 2: Every 5th call returns 503 Service Unavailable
            if (_coffeeService.IsOutOfCoffee())
            {
                return StatusCode(503);
            }

            // Requirement 1: Return 200 OK with JSON response
            var response = new CoffeeResponse
            {
                Message = "Your piping hot coffee is ready",
                Prepared = _coffeeService.GetCurrentTimestamp()
            };

            return Ok(response);
        }
    }
}