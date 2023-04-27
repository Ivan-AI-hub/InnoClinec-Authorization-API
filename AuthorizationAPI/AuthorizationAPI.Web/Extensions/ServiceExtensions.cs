using AuthorizationAPI.Application.Interfaces;
using AuthorizationAPI.DAL;
using AuthorizationAPI.DAL.Repositories;
using AuthorizationAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
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
                                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)));
        }
        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationContext, AuthorizationContext>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<AuthorizationService>();
            services.AddScoped<EmailService>();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration,
                                string jwtSettingsSection, string issuerSection, string audienceSection, string keySection)
        {
            var jwtSettings = configuration.GetSection(jwtSettingsSection);
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
                    ValidIssuer = jwtSettings.GetSection(issuerSection).Value,
                    ValidAudience = jwtSettings.GetSection(audienceSection).Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection(keySection).Value))
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
