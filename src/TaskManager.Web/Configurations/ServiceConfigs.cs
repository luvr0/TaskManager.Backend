using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaskManager.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, Microsoft.Extensions.Logging.ILogger logger, WebApplicationBuilder builder)
  {
    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatorSourceGen(logger)
            .AddAuthenticationAndAuthorization(builder.Configuration, logger);

    if (builder.Environment.IsDevelopment())
    {
      // Use a local test email server - configured in Aspire
      // See: https://ardalis.com/configuring-a-local-test-email-server/

      // Otherwise use this:
      //builder.Services.AddScoped<IEmailSender, FakeEmailSender>();
    }
    else
    {
    }

    logger.LogInformation("{Project} services registered", "Mediator Source Generator and Email Sender");

    return services;
  }

  private static IServiceCollection AddAuthenticationAndAuthorization(
    this IServiceCollection services,
    IConfiguration configuration,
    Microsoft.Extensions.Logging.ILogger logger)
  {
    var jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    var jwtIssuer = configuration["Jwt:Issuer"] ?? "TaskManager";
    var jwtAudience = configuration["Jwt:Audience"] ?? "TaskManager";

    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      };
    });

    services.AddAuthorization(options =>
    {
      // Only require authentication - authorization is handled at Board level
      options.AddPolicy("RequireAuthenticatedUser", policy => policy.RequireAuthenticatedUser());
    });

    logger.LogInformation("Authentication and Authorization configured");

    return services;
  }
}
