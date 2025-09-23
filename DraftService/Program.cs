using DraftService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connString = builder.Configuration["DB_CONN"]
                 ?? builder.Configuration.GetConnectionString("DB_CONN")
                 ?? "Host=localhost;Username=hh;Password=hh;Database=drafts";

builder.Services.AddDbContext<DraftDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddScoped<DraftRepository>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// Apply migrations on startup with retry
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DraftDbContext>();

    var retries = 5;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("DraftService DB migration applied successfully.");
            break; // success
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DB not ready yet: {ex.Message}");
            retries--;
            if (retries == 0)
            {
                Console.WriteLine("Failed to connect to DB after retries, exiting.");
                throw;
            }
            System.Threading.Thread.Sleep(5000); 
        }
    }
}

app.Run();
