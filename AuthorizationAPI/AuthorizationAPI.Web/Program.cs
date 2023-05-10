using AuthorizationAPI.Presentation.Controllers;
using AuthorizationAPI.Presentation.Settings;
using AuthorizationAPI.Services.Mappings;
using AuthorizationAPI.Services.Settings;
using AuthorizationAPI.Services.Validators;
using AuthorizationAPI.Web.Extensions;
using AuthorizationAPI.Web.Middlewares;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSqlContext(builder.Configuration, "DefaultConnection");

builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureServices();
builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(AuthorizationController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(ServiceMappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<SingUpValidator>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettingsConfig"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettingsConfig"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
