using AuthorizationAPI.Application.Commands.Users.Create;
using AuthorizationAPI.Application.Validators;
using AuthorizationAPI.Services.Models;
using AuthorizationAPI.Web.Extensions;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("JwtSettingsConfig.json");
builder.Configuration.AddJsonFile("EmailSettingsConfig.json");

builder.Services.ConfigureSqlContext(builder.Configuration, "DefaultConnection");

builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureServices();
builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUser).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<AddUserValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

builder.Services.Configure<JwtSettings>(builder.Configuration);
builder.Services.Configure<EmailSettings>(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
