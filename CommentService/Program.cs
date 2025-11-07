using CommentService.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
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
        t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CommentService"))
         .AddAspNetCoreInstrumentation()
         .AddHttpClientInstrumentation()
         .AddEntityFrameworkCoreInstrumentation()
         .AddOtlpExporter();
    });

var connString = builder.Configuration["DB_CONN"]
                 ?? "Host=localhost;Username=hh;Password=hh;Database=comments";

builder.Services.AddDbContext<CommentDbContext>(o => o.UseNpgsql(connString));
builder.Services.AddScoped<CommentRepository>();
builder.Services.AddSingleton<CommentCache>();

var profanityUrl = builder.Configuration["PROFANITY_URL"];
builder.Services.AddHttpClient("ProfanityService", c => c.BaseAddress = new Uri(profanityUrl))
    .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
        .CircuitBreakerAsync(1, TimeSpan.FromSeconds(30)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
