using ArticleService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Region resolver for multiple DBs
builder.Services.AddSingleton<RegionConnectionResolver>();

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// health check endpoint
app.MapGet("/whoami", () =>
{
    var instance = Environment.GetEnvironmentVariable("INSTANCE_NAME")
                   ?? Environment.GetEnvironmentVariable("HOSTNAME")
                   ?? Environment.MachineName;

    return Results.Ok(new { service = "ArticleService", instance });
});

app.Run();
