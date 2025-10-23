using DraftService.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341") 
    .CreateLogger();

try
{
    Log.Information("Starting DraftService...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    var connString = builder.Configuration["DB_CONN"]
                     ?? builder.Configuration.GetConnectionString("DB_CONN")
                     ?? "Host=localhost;Username=hh;Password=hh;Database=drafts";

    builder.Services.AddDbContext<DraftDbContext>(options =>
        options.UseNpgsql(connString));

    builder.Services.AddScoped<DraftRepository>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers();

    // Applyinf migrations on startup with retry + Serilog
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>();
        var retries = 5;

        while (retries > 0)
        {
            try
            {
                db.Database.Migrate();
                Log.Information("Database migration successful!");
                break;
            }
            catch (Exception ex)
            {
                retries--;
                Log.Warning(ex, "Database not ready, retries left: {Retries}", retries);
                if (retries == 0)
                {
                    Log.Fatal(ex, "Could not connect to DraftDatabase after retries");
                    throw;
                }
                Thread.Sleep(5000);
            }
        }
    }

    Log.Information("DraftService is running");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DraftService crashed unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
