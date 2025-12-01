using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VideoGamesLibrary.Api.OpenApi;
using VideoGamesLibrary.Application.Interfaces;
using VideoGamesLibrary.Application.Services;
using VideoGamesLibrary.Domain.Repositories;
using VideoGamesLibrary.Infrastructure.Data;
using VideoGamesLibrary.Infrastructure.Repositories;
using VideoGamesLibrary.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VideoGameLibraryDbContext>(options =>
    options.UseSqlite(connectionString));

// Jwt options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Authentification / JWT
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var keyBytes = Encoding.UTF8.GetBytes(jwtConfig.Key);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// DI métiers
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameRepository, EfGameRepository>();

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<VideoGameLibraryDbContext>();
    await DbInitializer.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Video Games API v1");
        // Optionnel : page d’accueil sur /swagger
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
