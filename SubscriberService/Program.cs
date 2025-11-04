using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SubscriberService.Data;
using SubscriberService.Models;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// --- DATABASE ---
builder.Services.AddDbContext<SubscriberDbContext>(opt =>
    opt.UseSqlite("Data Source=subscribers.db"));

// --- SWAGGER ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SubscriberService API",
        Version = "v1",
        Description = "Handles subscriber registration and posting to the queue."
    });
});

var app = builder.Build();

// --- SWAGGER UI CONFIGURATION ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SubscriberService v1");
        c.RoutePrefix = "swagger";
    });
}

// --- FEATURE TOGGLE ---
var isEnabled = builder.Configuration.GetValue<bool>("SubscriberServiceEnabled");

// --- READ QUEUE BASE URL FROM CONFIG ---
var queueBaseUrl = builder.Configuration.GetValue<string>("Queue:BaseUrl") ?? "http://localhost:5190";

if (isEnabled)
{
    app.MapPost("/api/subscribers", async (Subscriber subscriber, SubscriberDbContext db) =>
    {
        db.Subscribers.Add(subscriber);
        await db.SaveChangesAsync();

        using var http = new HttpClient();
        await http.PostAsJsonAsync($"{queueBaseUrl}/api/queue", new { email = subscriber.Email });

        return Results.Created($"/api/subscribers/{subscriber.Id}", subscriber);
    });

    app.MapGet("/api/subscribers", async (SubscriberDbContext db) =>
        await db.Subscribers.ToListAsync());

    app.MapDelete("/api/subscribers/{id}", async (int id, SubscriberDbContext db) =>
    {
        var subscriber = await db.Subscribers.FindAsync(id);
        if (subscriber is null) return Results.NotFound();

        db.Subscribers.Remove(subscriber);
        await db.SaveChangesAsync();
        return Results.Ok();
    });
}

app.Run();
