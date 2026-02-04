using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Speedrun.Models;
using Speedrun.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();









builder.Services.AddSingleton<IGameService, GameService>();


var app = builder.Build();


app.UseDefaultFiles();  // Serves index.html automatically
app.UseStaticFiles();   // Serves files from wwwroot


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
