using Microsoft.OpenApi.Models;
using SubscriberQueue.Models;

//http://localhost:5113/swagger

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SubscriberQueue API",
        Version = "v1",
        Description = "Queue where subscribers are stored temporarily before being processed."
    });
});

var app = builder.Build();

// In-memory queue
var queue = new List<SubscriberMessage>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add a new subscriber to the queue
app.MapPost("/api/queue", (SubscriberMessage message) =>
{
    message.Id = queue.Count + 1;
    message.CreatedAt = DateTime.UtcNow;
    queue.Add(message);
    return Results.Created($"/api/queue/{message.Id}", message);
});

// Get all queued subscribers
app.MapGet("/api/queue", () => Results.Ok(queue));

// Clear the queue (simulate consuming)
app.MapDelete("/api/queue", () =>
{
    queue.Clear();
    return Results.Ok("Queue cleared");
});

app.Run();
