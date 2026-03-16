using BookHistory.Factory;
using BookHistory.Entities.RepositoryContracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi;
using Prometheus;
using Prometheus.DotNetRuntime;
using Serilog;
using Serilog.Exceptions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionStrings = builder.Configuration.GetSection("ConnectionStrings").Get<Dictionary<string, string>>();
var factory = new RepositoryGenerator(connectionStrings);

// Add services to the container.
builder.Services.AddSingleton<IBookRepository>(factory.GetBookRepository());
builder.Services.AddSingleton<IBookHistoryRepository>(factory.GetBookHistoryRepository());

//Serilog logging
builder.Logging.ClearProviders();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
        options.JsonSerializerOptions.DefaultIgnoreCondition =
           JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookHistory API", Version = "v1" });
});

var app = builder.Build();

app.UseRouting();

// Collect HTTP request metrics
app.UseHttpMetrics();

// Collect .NET runtime metrics (GC, memory, threads)
DotNetRuntimeStatsBuilder.Default().StartCollecting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics(); // /metrics
});

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var message = "Unexpected exception";
        if (exceptionFeature?.Error != null)
        {
            try
            {
                Log.Error(exceptionFeature.Error, message);
            }
            catch
            {
            }

            message = exceptionFeature.Error.Message;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync(message);
    });
});

app.Run();
