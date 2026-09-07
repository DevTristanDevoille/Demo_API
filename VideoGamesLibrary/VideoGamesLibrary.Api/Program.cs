using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using VideoGamesLibrary.Application.Interfaces;
using VideoGamesLibrary.Application.Services;
using VideoGamesLibrary.Domain.Repositories;
using VideoGamesLibrary.Infrastructure.Data;
using VideoGamesLibrary.Infrastructure.Repositories;
using VideoGamesLibrary.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Document de base
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VideoGamesLibrary API",
        Version = "v1"
    });

    // Définition du schéma d’authentification Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. " +
                      "Collez uniquement le jeton, sans le préfixe \"Bearer\" : " +
                      "Swagger l'ajoute automatiquement.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Exigence de sécurité – NOUVELLE SYNTAXE .NET 10 / Swashbuckle 10
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        // La clé doit utiliser **exactement** le même nom que dans AddSecurityDefinition
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<VideoGamesLibraryDbContext>(options =>
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

// Hasher de mot de passe
builder.Services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

var app = builder.Build();

// Initialisation de la base de données : uniquement pour les environnements de démo.
// Le schéma n'est PAS créé ici en Development : il faut lancer "dotnet ef database update"
// avant le premier démarrage (voir le README).
// En Docker le conteneur part d'une base vide, la migration y est donc appliquée au démarrage.
// En environnement "Test", c'est la factory des tests d'intégration qui prend le relais.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<VideoGamesLibraryDbContext>();

    if (app.Environment.IsEnvironment("Docker"))
        await context.Database.MigrateAsync();

    await DbInitializer.SeedAsync(context);
}

// Documentation Swagger : exposée partout sauf en production.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();