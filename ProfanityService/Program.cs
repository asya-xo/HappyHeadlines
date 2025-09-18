using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;

namespace ProfanityService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Get connection string from environment variable (set in docker-compose)
            var connString = builder.Configuration["DB_CONN"]
                             ?? "Host=localhost;Username=hh;Password=hh;Database=profanity";

            // Register DbContext
            builder.Services.AddDbContext<ProfanityDbContext>(options =>
                options.UseNpgsql(connString));

            // Register repository directly (no interface)
            builder.Services.AddScoped<ProfanityRepository>();

            // Add controllers + Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Enable Swagger UI always (even in Docker/Production)
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.MapControllers();

            // Apply migrations with retry logic
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProfanityDbContext>();

                var retries = 5;
                while (retries > 0)
                {
                    try
                    {
                        db.Database.Migrate();
                        Console.WriteLine("Profanity DB migration applied.");
                        break;
                    }
                    catch (Exception ex)
                    {
                        retries--;
                        Console.WriteLine($"Could not connect to DB ({ex.Message}). Retries left: {retries}");
                        Thread.Sleep(2000); 
                        if (retries == 0) throw; 
                    }
                }
            }

            app.Run();
        }
    }
}
