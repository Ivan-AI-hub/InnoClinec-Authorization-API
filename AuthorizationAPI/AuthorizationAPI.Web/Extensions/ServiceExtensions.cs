using AuthorizationAPI.Application;
using AuthorizationAPI.Application.Abstractions;
using AuthorizationAPI.Application.Settings;
using AuthorizationAPI.Domain.Repositories;
using AuthorizationAPI.Persistence;
using AuthorizationAPI.Persistence.Repositories;
using AuthorizationAPI.Web.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using System;
using System.Text;

namespace AuthorizationAPI.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration, string connectionStringSectionName)
        {
            var connection = configuration.GetConnectionString(connectionStringSectionName);
            services.AddDbContext<AuthorizationContext>(options =>
                                options.UseSqlServer(connection,
                                b => b.MigrationsAssembly(typeof(AuthorizationContext).Assembly.GetName().Name)));
        }
        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationService, AuthorizationService>();
        }
        public static void ConfigureLogger(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment, string elasticUriSection)
        {
            services.AddSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration.Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration[elasticUriSection]))
                    {
                        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfShards = 2,
                        NumberOfReplicas = 1
                    })
                    .Enrich.WithProperty("Environment", environment.EnvironmentName)
                    .ReadFrom.Configuration(configuration);
            });
        }
        public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration, string massTransitSettingsName)
        {
            var settings = configuration.GetSection(massTransitSettingsName).Get<MassTransitSettings>();
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(settings.Host, settings.VirtualHost, h =>
                    {
                        h.Username(settings.UserName);
                        h.Password(settings.Password);
                    });
                    cfg.AddRawJsonSerializer();
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection(nameof(JwtSettings.ValidIssuer)).Value,
                    ValidAudience = configuration.GetSection(nameof(JwtSettings.ValidAudience)).Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection(nameof(JwtSettings.IssuerSigningKey)).Value))
                };
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                           Name = "Bearer",
                        },
                        new List<string>()
                    }
                });

            });
        }
    }
}
