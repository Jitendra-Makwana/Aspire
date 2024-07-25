using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(x => 
    {
        x.IncludeScopes = true;
        x.IncludeFormattedMessage = true;
        x.AddOtlpExporter(a => 
            {
                a.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
                a.Protocol = OtlpExportProtocol.HttpProtobuf;
                a.Headers = "X-Seq-ApiKey=SlW8CeWtWwWZibbEfz9Q";
            }
        );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () =>
{    
    return "Hello there!";
});

app.MapGet("/weatherforecast", (string city, int days, ILogger<Program> _logger) =>
{
    _logger.LogInformation("Entered into controller");
    var forecast =  Enumerable.Range(1, days).Select(index =>
        new WeatherForecast
        (
            city,
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    _logger.LogInformation("Retrived {WeatherCount} weather forcasts for {City}", forecast, city);
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(string city, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
