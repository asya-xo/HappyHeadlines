using System.Diagnostics;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Net.Http.Headers;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("ArticleService", client =>
{
    client.BaseAddress = new Uri("http://lb");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddOpenTelemetry()
    .WithTracing(t =>
    {
        t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PublisherService"))
         .AddAspNetCoreInstrumentation()
         .AddHttpClientInstrumentation()
         .AddOtlpExporter();
    });

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
