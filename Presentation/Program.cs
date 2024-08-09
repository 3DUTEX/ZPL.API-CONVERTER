using Application.Services;
using Domain.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Dependency Inversion
builder.Services.AddScoped<IZPLConverterService, ZPLConverterService>();

var app = builder.Build();

// swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
