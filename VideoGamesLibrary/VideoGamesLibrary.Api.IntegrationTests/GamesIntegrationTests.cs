using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VideoGamesLibrary.Application.Dtos;

namespace VideoGamesLibrary.Api.IntegrationTests;

public class TestDto
{
    public string Token { get; set; } = string.Empty;
}

public class GamesIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GamesIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllGames_ShouldReturn200AndSeededGame()
    {
        // Arrange : la base en mémoire est déjà créée et alimentée par la factory.

        // Act : sans jeton, l'API doit refuser l'accès
        var anonymousResponse = await _client.GetAsync("/api/games");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        // Act : on s'authentifie pour récupérer un jeton JWT
        var loginResponse = await _client.PostAsJsonAsync("/api/User/login", new LoginRequestDto
        {
            Username = "plop",
            Password = "plop"
        });

        loginResponse.EnsureSuccessStatusCode();

        var login = await loginResponse.Content.ReadFromJsonAsync<TestDto>();
        Assert.NotNull(login);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);

        // Act : le même appel, cette fois authentifié
        var authenticatedResponse = await _client.GetAsync("/api/games");

        // Assert
        authenticatedResponse.EnsureSuccessStatusCode();

        var games = await authenticatedResponse.Content.ReadFromJsonAsync<List<GameDto>>();
        Assert.NotNull(games);
        Assert.Contains(games, g => g.Title == "Elden Ring");
    }
}
