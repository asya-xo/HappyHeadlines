using ArticleService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Region resolver for multiple DBs
builder.Services.AddSingleton<RegionConnectionResolver>();

// Register repository
builder.Services.AddScoped<ArticleRepository>();

// Add controllers
builder.Services.AddControllers();

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<ArticleCacheLoader>();
builder.Services.AddSingleton<ArticleCache>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Health check
app.MapGet("/whoami", () =>
{
    var instance = Environment.GetEnvironmentVariable("INSTANCE_NAME")
                   ?? Environment.GetEnvironmentVariable("HOSTNAME")
                   ?? Environment.MachineName;

    return Results.Ok(new { service = "ArticleService", instance });
});

// Apply EnsureCreated on all region DBs
using (var scope = app.Services.CreateScope())
{
    var resolver = scope.ServiceProvider.GetRequiredService<RegionConnectionResolver>();
    foreach (var region in new[] { "global", "eu", "na", "sa", "af", "as", "oc", "an" })
    {
        try
        {
            using var ctx = ArticleDbContext.Create(resolver.GetConnectionString(region));
            Console.WriteLine($"[ArticleService] DB ready for {region}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ArticleService] Failed for {region}: {ex.Message}");
        }
    }
}

app.Run();
