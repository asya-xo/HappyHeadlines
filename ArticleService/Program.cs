using ArticleService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (use env var or fallback default)
var connString = builder.Configuration["DB_CONN"]
                 ?? "Host=localhost;Username=hh;Password=hh;Database=articles";

builder.Services.AddDbContext<ArticleDbContext>(options =>
    options.UseNpgsql(connString));

// DI for repository
builder.Services.AddScoped<ArticleRepository>();

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

// --- Auto-migrate on startup with retry ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();

    var retries = 5;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("[ArticleService] DB migration applied successfully.");
            break; // success
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"[ArticleService] Could not connect to DB ({ex.Message}). Retries left: {retries}");
            Thread.Sleep(2000);
            if (retries == 0) throw;
        }
    }
}

app.Run();
