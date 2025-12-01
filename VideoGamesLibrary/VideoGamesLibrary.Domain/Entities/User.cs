namespace VideoGamesLibrary.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    // Pour la démo, mot de passe en clair ou pseudo-haché.
    // À expliquer clairement : en vrai projet, on ne stocke JAMAIS le mot de passe en clair.
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User";
}
