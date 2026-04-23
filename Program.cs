using CoffeeMachineAPI.Interfaces;
using CoffeeMachineAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICoffeeService, CoffeeService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddHttpClient<IWeatherService, WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

public partial class Program { }