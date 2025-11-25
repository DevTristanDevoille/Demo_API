using Microsoft.EntityFrameworkCore;
using VideoGamesLibrary.Application.Interfaces;
using VideoGamesLibrary.Application.Services;
using VideoGamesLibrary.Domain.Repositories;
using VideoGamesLibrary.Infrastructure.Data;
using VideoGamesLibrary.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// DbContext EF Core (SQLite)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VideoGameLibraryDbContext>(options =>
    options.UseSqlite(connectionString));

// Dependency Injection
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameRepository, EfGameRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
