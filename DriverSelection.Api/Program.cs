using DriverSelection.Api.Configuration;
using DriverSelection.Api.Services;
using DriverSelection.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MapSettings>(builder.Configuration.GetSection("MapSettings"));
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));

builder.Services.AddSingleton<DriverStorage>();

builder.Services.AddHttpClient<RandomNumberService>(client => {
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<RequestLimitMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
