var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorPages();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseHttpsRedirection();

//app.UseDefaultFiles();  // Serves index.html automatically
app.UseStaticFiles();   // Serves files from wwwroot

// Use routing
app.UseRouting();

// Configure the HTTP request pipeline.s
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/Games");
    return Task.CompletedTask;
});

app.Run();
