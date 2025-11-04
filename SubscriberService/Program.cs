using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SubscriberService.Data;
using SubscriberService.Models;

// https://localhost:7084/swagger/index.html

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddDbContext<SubscriberDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

var isEnabled = builder.Configuration.GetValue<bool>("SubscriberServiceEnabled", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SubscriberService v1");
    });
}

if (isEnabled)
{
    // Database-backed endpoints
    app.MapPost("/api/subscribers", async (Subscriber newSub, SubscriberDbContext db) =>
    {
        newSub.CreatedAt = DateTime.UtcNow;
        db.Subscribers.Add(newSub);
        await db.SaveChangesAsync();
        return Results.Created($"/api/subscribers/{newSub.Id}", newSub);
    });

    app.MapGet("/api/subscribers", async (SubscriberDbContext db) =>
    {
        var allSubs = await db.Subscribers.ToListAsync();
        return Results.Ok(allSubs);
    });
}
else
{
    // Service disabled (release toggle)
    app.MapGet("/{*path}", () => Results.StatusCode(503));
    app.MapPost("/{*path}", () => Results.StatusCode(503));
}

app.Run();
