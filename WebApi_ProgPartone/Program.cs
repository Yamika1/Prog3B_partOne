using Microsoft.EntityFrameworkCore;
using WebApi_ProgPartone.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WebApi_ProgPartoneContext") ?? throw new InvalidOperationException("Connection string 'WebApi_ProgPartoneContext' not found.");


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApi_ProgPartoneContext")));

//builder.Services.AddHttpClient("SensorApi", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7101/api/");
//});

builder.Services.AddSwaggerGen();

var app = builder.Build();

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
