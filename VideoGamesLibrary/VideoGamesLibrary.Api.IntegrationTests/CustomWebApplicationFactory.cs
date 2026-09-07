using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using VideoGamesLibrary.Infrastructure.Data;

namespace VideoGamesLibrary.Api.IntegrationTests;

/// <summary>
/// Démarre l'API en mémoire pour les tests, en remplaçant la base SQLite
/// sur fichier par une base SQLite en mémoire, créée et alimentée une fois
/// pour toutes avant l'exécution du premier test.
/// </summary>
public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // On retire l'enregistrement du DbContext défini dans Program.cs
            // (SQLite sur fichier) pour le remplacer par le nôtre.
            RemoveService(services, typeof(IDbContextOptionsConfiguration<VideoGameLibraryDbContext>));
            RemoveService(services, typeof(DbConnection));

            // La connexion est ouverte et enregistrée en singleton : SQLite détruit
            // une base ":memory:" dès que sa dernière connexion se ferme.
            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<VideoGameLibraryDbContext>((container, options) =>
                options.UseSqlite(container.GetRequiredService<DbConnection>()));
        });

        builder.UseEnvironment("Test");
    }

    /// <summary>
    /// Crée le schéma et injecte les données de test avant que le premier test
    /// n'émette une requête. Placé ici, et non dans une méthode de test, le setup
    /// ne dépend pas de l'ordre d'exécution des tests.
    /// </summary>
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<VideoGameLibraryDbContext>();

        context.Database.EnsureCreated();
        DbInitializer.SeedAsync(context).GetAwaiter().GetResult();

        return host;
    }

    private static void RemoveService(IServiceCollection services, Type serviceType)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == serviceType);

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }
}
