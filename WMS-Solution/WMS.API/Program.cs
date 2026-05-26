using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using WMS.API.Extensions;
using WMS.Application.Validators.Employee;
using WMS.Infrastructure.Persistence;
using WMS.Infrastructure.Seeder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License =
    LicenseType.Community;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build())
    .CreateLogger();

var builder =
    WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngularApp",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200",
                    "https://wms-frontend-prasanna-bgf0gyddfghha8c0.centralindia-01.azurewebsites.net"
                )

                .AllowAnyHeader()

                .AllowAnyMethod();
        });
});

builder.Services
    .AddControllers();

builder.Services
    .AddFluentValidationAutoValidation();

builder.Services
    .AddValidatorsFromAssemblyContaining<
        CreateEmployeeDtoValidator>();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "WMS API",
            Version = "v1"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "Bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
                "Enter JWT Token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

var jwtSettings =
    builder.Configuration.GetSection("JwtSettings");

var secretKey =
    jwtSettings["Secret"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })

    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],

                ValidAudience = jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            secretKey!))
            };
    });

var app = builder.Build();


app.UseSwagger();

app.UseSwaggerUI();

app.UseGlobalExceptionMiddleware();

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuditMiddleware();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services
        .GetRequiredService<WmsDbContext>();

    if (app.Environment.IsDevelopment())
    {
        await DbSeeder.SeedAsync(context);
        Log.Information("Database seeding completed.");
    }
}

app.Run();
