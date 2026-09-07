using VideoGamesLibrary.Domain.Entities;

namespace VideoGamesLibrary.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(VideoGamesLibraryDbContext context)
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
                Password = "AQAAAAIAAYagAAAAEOy3MCKWzNWqdYgFsR+W994/T6r/v/9oYtUxw2I1PyaRuaikze+WghIlAlmTOTCdWw==",
                Role = "Admin",
                Email = "admin@admin.fr"
            },
            new User
            {
                Username = "user",
                Password = "AQAAAAIAAYagAAAAEPb4K0wPcZn6lO6XvGY7i6V6eJaCJg07KmTyap+ON+q6pHkBv6OaOWqJNbwEhheZ/Q==",
                Role = "User",
                Email = "user@user.fr"
            },
            new User
            {
                Username = "plop",
                Password = "AQAAAAIAAYagAAAAEBykYTlNc3YsMik7/KjHmG+LnTtIMFsy3GkK5iFMguA3OT3ruzDSCI4O8f/eikhtEA==",
                Role = "User",
                Email = "plop@plop.fr"
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
