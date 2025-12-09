using VideoGamesLibrary.Domain.Entities;

namespace VideoGamesLibrary.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(VideoGameLibraryDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                Password = "password",
                Role = "Admin",
                Email = "admin@admin.fr",
                Salt = "a1f4s5fe4"
            },
            new User
            {
                Username = "user",
                Password = "password",
                Role = "User",
                Email = "user@user.fr",
                Salt = "9f8g7h6j5k",
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        if (context.Games.Any())
        {
            return;
        }

        var games = new List<Game>
        {
            new Game
            {
                Title = "The Legend of Zelda: Breath of the Wild",
                Platform = "Nintendo Switch",
                Genre = "Action-Adventure",
                ReleaseDate = new DateTime(2017, 3, 3)
            },
            new Game
            {
                Title = "Elden Ring",
                Platform = "PC",
                Genre = "Action-RPG",
                ReleaseDate = new DateTime(2022, 2, 25)
            }
        };

        context.Games.AddRange(games);
        await context.SaveChangesAsync();
    }
}
