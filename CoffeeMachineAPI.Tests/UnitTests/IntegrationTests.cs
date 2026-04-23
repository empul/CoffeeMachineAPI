using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.Tests;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BrewCoffee_Returns200Or503Or418()
    {
        // Act
        var response = await _client.GetAsync("/coffee/brew-coffee");

        // Assert
        var statusCode = (int)response.StatusCode;
        Assert.True(
            statusCode == 200 || statusCode == 503 || statusCode == 418,
            $"Unexpected status code: {statusCode}"
        );
    }

    [Fact]
    public async Task BrewCoffee_ReturnsValidJson_WhenOk()
    {
        // Act
        var response = await _client.GetAsync("/coffee/brew-coffee");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<CoffeeResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(json);
            Assert.Equal("Your piping hot coffee is ready", json.Message);
            Assert.Contains("T", json.Prepared); // ISO format check
        }
    }

    [Fact]
    public async Task BrewCoffee_Returns418_OnAprilFirst()
    {
        var response = await _client.GetAsync("/coffee/brew-coffee");

        if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
        {
            Assert.Equal(418, (int)response.StatusCode);
        }
        else
        {
            Assert.True((int)response.StatusCode == 200 || (int)response.StatusCode == 503);
        }
    }

    [Fact]
    public async Task BrewCoffee_ReturnsEmptyBody_ForErrors()
    {
        var response = await _client.GetAsync("/coffee/brew-coffee");

        // If it's an error status, body should be empty
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Empty(content);
        }
    }
}