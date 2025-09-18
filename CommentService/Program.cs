using Microsoft.EntityFrameworkCore;
using CommentService.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection string (env > appsettings > fallback localhost)
var connString = builder.Configuration["DB_CONN"]
                 ?? builder.Configuration.GetConnectionString("DB_CONN")
                 ?? "Host=localhost;Username=hh;Password=hh;Database=comments";

builder.Services.AddDbContext<CommentDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddScoped<CommentRepository>();

// ProfanityService URL (env > appsettings > fallback localhost)
// Important: don't end with /check, just the base path
var profanityUrl = builder.Configuration["PROFANITY_URL"]
                   ?? builder.Configuration["ProfanityService:BaseUrl"]
                   ?? "http://localhost:5002/api/profanity/";

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

app.Run();
