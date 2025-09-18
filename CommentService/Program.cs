using Microsoft.EntityFrameworkCore;
using CommentService.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection stringg
var connString = builder.Configuration["DB_CONN"]
                 ?? builder.Configuration.GetConnectionString("DB_CONN")
                 ?? "Host=localhost;Username=hh;Password=hh;Database=comments";

builder.Services.AddDbContext<CommentDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddScoped<CommentRepository>();

// ProfanityService URL 
var profanityUrl = builder.Configuration["PROFANITY_URL"];


builder.Services.AddHttpClient("ProfanityService", client =>
{
    client.BaseAddress = new Uri(profanityUrl);
});

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
    var db = scope.ServiceProvider.GetRequiredService<CommentDbContext>();

    var retries = 5;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("CommentService DB migration applied successfully.");
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
            System.Threading.Thread.Sleep(5000); // wait 5 sec before retry
        }
    }
}

app.Run();
