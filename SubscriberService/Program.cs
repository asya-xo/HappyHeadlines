using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SubscriberService.Models;

//https://localhost:7084/swagger/index.html

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<SubscriberService.Data.SubscriberDbContext>(options =>
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
}
else
{
    app.MapGet("/{*path}", () => Results.StatusCode(503));
    app.MapPost("/{*path}", () => Results.StatusCode(503));
}


app.Run();
