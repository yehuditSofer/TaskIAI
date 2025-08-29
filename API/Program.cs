using Common.InterfacesBL;
using BL;
using Common.InterfacesDAL;
using DAL.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = "346337221074-i7uucmn496skt0mpecv2j46t63lmpnal.apps.googleusercontent.com", // äÎClientId ùìê
            ValidateLifetime = true
        };
    });

builder.Services.AddControllers();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "BoardApp API is running");

app.Run();