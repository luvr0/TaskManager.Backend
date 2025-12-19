using TaskManager.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()    // This sets up OpenTelemetry logging
       .AddLoggerConfigs();     // This adds Serilog for console formatting

// Temporary solution for CORS during local development
var allowedOrigins = new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowLocalDev", policy =>
  {
    policy.WithOrigins(allowedOrigins)
          .AllowCredentials()
          .AllowAnyHeader()
          .AllowAnyMethod();
  });
});
// End temporary CORS solution

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddOptionConfigs(builder.Configuration, startupLogger, builder);
builder.Services.AddServiceConfigs(startupLogger, builder);

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });

var app = builder.Build();

await app.UseAppMiddlewareAndSeedDatabase();

app.MapDefaultEndpoints(); // Aspire health checks and metrics

app.Run();

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program { }
