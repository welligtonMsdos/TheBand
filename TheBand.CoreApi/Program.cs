using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using Scalar.AspNetCore;
using System.Text;
using TheBand.CoreApi.Middleware;
using TheBand.CoreApplication.Extensions;
using TheBand.CoreInfrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load(Path.Combine(builder.Environment.ContentRootPath, ".env"));

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://theband-auth.onrender.com",
                           "http://localhost:4200",
                           "https://tom-colections.onrender.com")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials();
    });
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        if (!builder.Environment.IsDevelopment())
        {
            document.Servers =
            [
                new() { Url = "https://theband-qv3s.onrender.com" }
            ];
        }

        return Task.CompletedTask;
    });
});

builder.Services.AddCoreApplication();
builder.Services.AddCoreInfrastructure(builder.Configuration);
var jwtKey = builder.Configuration["JwtSettings:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException("JWT key is missing or too short (minimum 32 characters).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;       
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = "https://theband-auth.onrender.com",
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

app.UseForwardedHeaders(); 

app.UseMiddleware<VinylValidationMiddleware>();
app.UseMiddleware<CassetteValidationMiddleware>();
app.UseMiddleware<ConcertValidationMiddleware>();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.Title = "TheBand API Reference";
    options.Theme = ScalarTheme.BluePlanet;
    options.DefaultHttpClient = new(ScalarTarget.JavaScript, ScalarClient.HttpClient);
    options.CustomCss = "";
    options.ShowSidebar = true;
    options.DarkMode = true;
    options.AddPreferredSecuritySchemes("Bearer")
           .AddHttpAuthentication("Bearer", auth =>
           {
               auth.Token = "your-bearer-token";
           });
});

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



