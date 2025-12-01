using VideoGamesLibrary.Domain.Entities;

namespace VideoGamesLibrary.Application.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}
