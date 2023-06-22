using AuthorizationAPI.Application.Mappings;
using AuthorizationAPI.Application.Settings;
using AuthorizationAPI.Application.Validators;
using AuthorizationAPI.Persistence;
using AuthorizationAPI.Presentation.Controllers;
using AuthorizationAPI.Web.Extensions;
using AuthorizationAPI.Web.Middlewares;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSqlContext(builder.Configuration, "DefaultConnection");
builder.Services.ConfigureLogger(builder.Configuration, builder.Environment, "ElasticConfiguration:Uri");

builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureServices();
builder.Services.ConfigureMassTransit(builder.Configuration, "MassTransitSettings");
builder.Services.ConfigureJWT(builder.Configuration.GetSection("JwtSettingsConfig"));

builder.Services.AddControllers()
    .AddApplicationPart(typeof(AuthorizationController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(ServiceMappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<SingUpValidator>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettingsConfig"));
builder.Services.Configure<AuthorizationSettings>(builder.Configuration.GetSection("AuthorizationSettingsConfig"));

var app = builder.Build();
app.MigrateDatabase<AuthorizationContext>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
