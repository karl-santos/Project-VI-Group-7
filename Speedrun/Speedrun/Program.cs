using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Speedrun.Models;
using Speedrun.Services;
using SQLitePCL;

var builder = WebApplication.CreateBuilder(args);

Batteries.Init();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IRunService, RunService>();
builder.Services.AddSingleton<ICommentService, CommentService>();



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


app.UseDefaultFiles();  // Serves index.html automatically
app.UseStaticFiles();   // Serves files from wwwroot


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors("AllowAll");

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
