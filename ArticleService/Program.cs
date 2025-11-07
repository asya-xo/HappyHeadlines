using ArticleService.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Serilog;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddOpenTelemetry()
    .WithTracing(t =>
    {
        t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ArticleService"))
         .AddAspNetCoreInstrumentation()
         .AddEntityFrameworkCoreInstrumentation()
         .AddOtlpExporter();
    });

// Swagger + services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RegionConnectionResolver>();
builder.Services.AddScoped<ArticleRepository>();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHostedService<ArticleCacheLoader>();
builder.Services.AddSingleton<ArticleCache>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.MapGet("/whoami", () =>
{
    var instance = Environment.GetEnvironmentVariable("INSTANCE_NAME")
                   ?? Environment.GetEnvironmentVariable("HOSTNAME")
                   ?? Environment.MachineName;
    return Results.Ok(new { service = "ArticleService", instance });
});

using (var scope = app.Services.CreateScope())
{
    var resolver = scope.ServiceProvider.GetRequiredService<RegionConnectionResolver>();
    foreach (var region in new[] { "global", "eu", "na", "sa", "af", "as", "oc", "an" })
    {
        try
        {
            using var ctx = ArticleDbContext.Create(resolver.GetConnectionString(region));
            Console.WriteLine($"[ArticleService] DB ready for {region}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ArticleService] Failed for {region}: {ex.Message}");
        }
    }
}
app.Run();
