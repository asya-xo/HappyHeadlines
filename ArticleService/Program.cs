using ArticleService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI singletons
builder.Services.AddSingleton<RegionConnectionResolver>();
builder.Services.AddSingleton<ArticleRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// simple endpoint to see which instance served the request
app.MapGet("/whoami", () =>
{
    var instance = Environment.GetEnvironmentVariable("INSTANCE_NAME")
                   ?? Environment.GetEnvironmentVariable("HOSTNAME")
                   ?? Environment.MachineName;

    return Results.Ok(new { instance });
});


app.Run();
