using api.mapping;
using api.middleware;
using AutoMapper;
using core.Entities;
using core.Services;
using inftastructer;
using inftastructer.Repository.Services;
using StackExchange.Redis;

using System.Reflection;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddUserSecrets<Program>();
builder.Services.InfrastructureConfiguration(builder.Configuration, builder.Environment.ContentRootPath);

builder.Services.AddScoped<ICustomerBasketService, CustomerBasketService>();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddScoped<core.interfaces.IBasketRepository, inftastructer.Repository.CustomerBasketRepository>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

// AutoMapper registration (explicit) to ensure IMapper is available for DI

builder.Services.AddSingleton<IMapper>(sp =>
{
    var config = new MapperConfiguration(cfg => cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()));
    return config.CreateMapper();
});

var app = builder.Build();

app.UseMiddleware<ExceptionsMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger";
    });

    app.MapGet("/", () => Results.Redirect("/swagger"));

    app.MapGet("/docs", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync("""
<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>Shopping API - Stoplight Elements</title>
    <link rel="stylesheet" href="https://unpkg.com/@stoplight/elements/styles.min.css">
    <script src="https://unpkg.com/@stoplight/elements/web-components.min.js"></script>
    <style>
      body {
        height: 100vh;
        margin: 0;
        overflow: hidden;
      }
    </style>
  </head>
  <body>
    <elements-api
      apiDescriptionUrl="/swagger/v1/swagger.json"
      router="hash"
      layout="sidebar"
    />
  </body>
</html>
""");
    });
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();