using Microsoft.EntityFrameworkCore;
using ProfanityService.Data;
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
        t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ProfanityService"))
         .AddAspNetCoreInstrumentation()
         .AddEntityFrameworkCoreInstrumentation()
         .AddOtlpExporter();
    });

var connString = builder.Configuration["DB_CONN"]
                 ?? "Host=localhost;Username=hh;Password=hh;Database=profanity";

builder.Services.AddDbContext<ProfanityDbContext>(o => o.UseNpgsql(connString));
builder.Services.AddScoped<ProfanityRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
