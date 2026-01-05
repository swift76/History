using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.PostgreRepositories;
using Genetec.BookHistory.SQLRepositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi;
using Prometheus;
using Prometheus.DotNetRuntime;
using Serilog;
using Serilog.Exceptions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var sqlServerConnectionString = builder.Configuration.GetConnectionString("SQLServer");
var postgreSQLConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");

//If SQLServer connection string exists and is filled, then SQL+Dapper implementation will work
if (!string.IsNullOrWhiteSpace(sqlServerConnectionString))
{
    builder.Services.AddSingleton<IBookRepository>(new DapperBookRepository(sqlServerConnectionString));
    builder.Services.AddSingleton<IBookHistoryRepository>(new DapperBookHistoryRepository(sqlServerConnectionString));
}
//Otherwise, if PostgreSQL connection string exists and is filled, then PostGre+EF implementation will work
else if (!string.IsNullOrWhiteSpace(postgreSQLConnectionString))
{
    builder.Services.AddSingleton<IBookRepository>(new EFBookRepository(postgreSQLConnectionString));
    builder.Services.AddSingleton<IBookHistoryRepository>(new EFBookHistoryRepository(postgreSQLConnectionString));
}
else
{
    throw new Exception("Connection string is not specified");
}

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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Genetec BookHistory API", Version = "v1" });
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
