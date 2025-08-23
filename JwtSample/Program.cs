using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using JwtSample.Services;
using JwtSample.EndPoints;
using JwtSample.Models;
using JwtSample.Data;
using JwtSample.Repositories;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

builder.Services.AddOptions<JwtConfig>()
     .Bind(builder.Configuration.GetSection("JwtConfig"))
     .ValidateDataAnnotations()
     .ValidateOnStart(); 

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtConfig>>().Value);

builder.Services.AddScoped<RefreshTokenRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();

// Only for database 
var connString = builder.Configuration.GetConnectionString("JwtDbConnection");
if (string.IsNullOrEmpty(connString))
{
    throw new Exception("Connection string not found in appsettings.json");
}

builder.Services.AddSqlite<JwtSampleDbContext>(connString);

// Configure authentication + JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>()
            ?? throw new InvalidOperationException("JwtConfig section is missing or invalid.");

        if (string.IsNullOrWhiteSpace(jwtConfig.Issuer) ||
            string.IsNullOrWhiteSpace(jwtConfig.Audience) ||
            string.IsNullOrWhiteSpace(jwtConfig.Key))
        {
            throw new InvalidOperationException("JWT configuration is incomplete. Please check appsettings.json.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig.Key))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                logger.LogWarning("JWT authentication failed: {Message}", context.Exception.Message);

                // NO RESPONSE WRITE HERE!
                // Let OnChallenge handle the response

                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse(); // Suppress the default response

                string errorCode;
                string errorMessage = "Authentication failed due to an invalid or expired token.";

                var ex = context.AuthenticateFailure;
                switch (ex)
                {
                    case SecurityTokenExpiredException:
                        errorCode = "TokenExpired";
                        errorMessage = "Authentication failed because the token has expired.";
                        break;
                    case SecurityTokenInvalidSignatureException:
                        errorCode = "InvalidSignature";
                        break;
                    case SecurityTokenInvalidIssuerException:
                        errorCode = "InvalidIssuer";
                        break;
                    case SecurityTokenInvalidAudienceException:
                        errorCode = "InvalidAudience";
                        break;
                    case SecurityTokenNotYetValidException:
                        errorCode = "TokenNotYetValid";
                        errorMessage = "Authentication failed because the token is not yet valid.";
                        break;
                    default:
                        errorCode = "InvalidToken";
                        break;
                }

                var result = JsonSerializer.Serialize(new
                {
                    error = errorCode,
                    message = errorMessage
                });

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(result);
            }
        };
    });


builder.Services.AddScoped<IUserRepository,UserRepository>();

builder.Services.AddScoped<UserService>();

builder.Services.AddAuthorization();

var app = builder.Build();

//  Log right before the app starts
app.Logger.LogInformation("App is starting...");



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapWeatherForecast();
app.MapUser();
app.MapLogin();


app.Run();