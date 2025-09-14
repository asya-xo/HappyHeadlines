using CommentService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database connection
var connString = builder.Configuration["DB_CONN"];
builder.Services.AddDbContext<CommentDbContext>(options =>
    options.UseNpgsql(connString));

// DI for repository
builder.Services.AddScoped<CommentRepository>();

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

    return Results.Ok(new { service = "CommentService", instance });
});

app.Run();
