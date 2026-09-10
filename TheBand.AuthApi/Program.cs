using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using TheBand.AuthApi.Exceptions;
using TheBand.AuthApplication.Extensions;
using TheBand.AuthApplication.Validators;
using TheBand.AuthInfrastructure.Data;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

#region 1. CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5001", "http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

#endregion

#region 2. Validação e Tratamento de Erros

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

#endregion

#region 3. Camadas de Aplicação e Infraestrutura

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

#endregion

#region 4. OpenAPI / Scalar

builder.Services.AddOpenApi();

#endregion

#region 5. Autenticação JWT e Autorização

var secret = builder.Configuration["JwtSettings:Key"];

if (string.IsNullOrEmpty(secret) || secret.Length < 32)
    throw new InvalidOperationException("JWT key is missing or too short (minimum 32 characters).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:5001",
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

#endregion

var app = builder.Build();

#region PIPELINE DE EXECUÇÃO

app.UseExceptionHandler();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("TheBand API Reference")
        .WithTheme(ScalarTheme.BluePlanet)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.MapControllers();

app.Run();

#endregion