using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using Scalar.AspNetCore;
using System.Text;
using TheBand.CoreApi.Filters;
using TheBand.CoreApi.Middleware;
using TheBand.CoreApplication.Extensions;
using TheBand.CoreInfrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load(Path.Combine(builder.Environment.ContentRootPath, ".env"));

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

builder.Services.AddOpenApi();
builder.Services.AddCoreApplication();
builder.Services.AddCoreInfrastructure(builder.Configuration);
builder.Services.AddScoped<UserIdMatchesTokenFilter>();

var jwtKey = builder.Configuration["JwtSettings:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException("JWT key is missing or too short (minimum 32 characters).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:5001",
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
        .WithTitle("TheBand API Reference")
        .WithTheme(ScalarTheme.Moon)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<VinylValidationMiddleware>();
app.MapControllers();

app.Run();
