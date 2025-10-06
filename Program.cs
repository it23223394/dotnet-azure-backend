var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",  // Local development
            "https://react-frontend-yourname.xxx.azurestaticapps.net"  // Your deployed React app
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Keep OpenAPI if you want Swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi(); // Keep for Swagger in development
}

// Use CORS policy - MUST be before UseRouting and UseAuthorization
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Your existing weather forecast endpoint (optional - you can remove if using controllers)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}