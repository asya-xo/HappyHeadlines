using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SubscriberService.Models;

//https://localhost:7084/swagger/index.html

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SubscriberService v1");
    });
}

var subscribers = new List<Subscriber>();

app.MapPost("/api/subscribers", (Subscriber newSub) =>
{
    newSub.Id = subscribers.Count + 1;
    newSub.CreatedAt = DateTime.UtcNow;
    subscribers.Add(newSub);
    return Results.Created($"/api/subscribers/{newSub.Id}", newSub);
});

app.MapGet("/api/subscribers", () =>
{
    return Results.Ok(subscribers);
});

app.Run();


