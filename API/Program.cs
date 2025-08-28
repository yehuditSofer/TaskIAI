using Common.InterfacesBL;
using BL;
using Common.InterfacesDAL;
using DAL.Json;
using Common.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Angular dev server
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "ads.json");
Directory.CreateDirectory(Path.GetDirectoryName(dataPath)!);

builder.Services.AddSingleton<IAdRepository>(sp => new JsonAdRepository(dataPath));
builder.Services.AddScoped<IAdService, AdService>();

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapGet("/", () => "BoardApp API is running");

app.Run();