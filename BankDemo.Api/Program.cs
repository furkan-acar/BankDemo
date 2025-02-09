using MediatR;
using Microsoft.EntityFrameworkCore;
using BankDemo.Infrastructure.Persistence;
using BankDemo.Application.Commands;
using BankDemo.Application.Handlers;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Reflection;
using BankDemo.Api;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Formatting.Compact;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var elasticUri = builder.Configuration["ElasticConfiguration:Uri"];
if (string.IsNullOrEmpty(elasticUri))
{
    throw new ArgumentNullException("ElasticConfiguration:Uri is required");
}

//serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    // .WriteTo.File(new RenderedCompactJsonFormatter(), "logs/log.json", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
    {
        IndexFormat = $"bankdemo-logs-{builder.Environment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        AutoRegisterTemplate = true,
        NumberOfShards = 2,
        NumberOfReplicas = 1
    })
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
    
builder.Services.AddMediatR(typeof(CreateAccountCommandHandler).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Bank Demo API", 
        Version = "v1"
    });
});

builder.Services.AddExceptionHandler<AppNotImplementedExceptionHandler>();
builder.Services.AddExceptionHandler<AppExceptionHandler>();

builder.Services.AddDbContext<BankDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        //    .EnableSensitiveDataLogging()
        //    .LogTo(Console.WriteLine, LogLevel.Debug);
});

builder.Services.AddScoped<BankDemo.Domain.Account.IAccountRepository, BankDemo.Infrastructure.Repositories.AccountRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(_ => { });

// app.UseHttpsRedirection();
app.UseAuthorization();
app.Urls.Add("http://0.0.0.0:8080");
app.MapControllers();
app.Run();
