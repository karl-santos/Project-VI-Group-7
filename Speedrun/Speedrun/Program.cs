using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Speedrun.Data;
using Speedrun.Models;
using Speedrun.Services;
using SQLitePCL;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Batteries.Init();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignore circular references when serializing to JSON
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Tell the app to use SQLite as the database
builder.Services.AddDbContext<SpeedrunDbContext>(options =>
    options.UseSqlite("Data Source=speedrun.db"));


// Register services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IRunService, RunService>();
builder.Services.AddScoped<ICommentService, CommentService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});



var app = builder.Build();


// create database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SpeedrunDbContext>();
    db.Database.EnsureCreated(); // checks if the database exists, if not it creates it
}





app.UseDefaultFiles();  // Serves index.html automatically
app.UseStaticFiles();   // Serves files from wwwroot


// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
