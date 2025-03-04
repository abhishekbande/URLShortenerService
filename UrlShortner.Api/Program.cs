using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using UrlShortener.Services;
using UrlShortener.Services.Interfaces;
using UrlShortner.Api.Middleware;
using UrlShortner.Api.Validators;
using UrlShortner.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddValidatorsFromAssemblyContaining<UrlRequestValidator>(); // FluentValidation setup

// Add services to the container.
builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();
builder.Services.AddSingleton<IUrlRepository, InMemoryUrlRepository>();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UrlRequestValidator>());

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Include XML comments
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "UrlShortner.Api.xml");
    c.IncludeXmlComments(xmlFile);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS setup
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use the exception handling middleware
app.UseExceptionHandlingMiddleware();

// Enable Swagger middleware
app.UseSwagger();

// Enable Swagger UI middleware
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "URL Shortner API V1");
    c.RoutePrefix = string.Empty;  // Makes Swagger UI accessible at the root URL (optional)
});

// CORS middleware
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
