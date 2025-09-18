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

            // MAking sure database is created / migrated
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProfanityDbContext>();
                db.Database.EnsureCreated(); 
            }


            app.Run();
        }
    }
}
