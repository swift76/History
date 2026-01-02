using Genetec.BookHistory.Entities.RepositoryContracts;
using Genetec.BookHistory.SQLRepositories;
using Genetec.BookHistory.PostgreRepositories;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var sqlServerConnectionString = builder.Configuration.GetConnectionString("SQLServer");
var postgreSQLConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");
if (!string.IsNullOrWhiteSpace(sqlServerConnectionString))
{
    builder.Services.AddSingleton<IBookRepository>(new BookRepository(sqlServerConnectionString));
    builder.Services.AddSingleton<IBookHistoryRepository>(new BookHistoryRepository(sqlServerConnectionString));
}
else if (!string.IsNullOrWhiteSpace(postgreSQLConnectionString))
{
    builder.Services.AddSingleton<IBookRepository>(new EFBookRepository(postgreSQLConnectionString));
    builder.Services.AddSingleton<IBookHistoryRepository>(new EFBookHistoryRepository(postgreSQLConnectionString));
}
else
{
    throw new Exception("Connection string is not specified");
}

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
