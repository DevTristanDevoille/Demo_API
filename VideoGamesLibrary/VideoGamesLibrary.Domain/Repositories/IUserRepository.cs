using VideoGamesLibrary.Domain.Entities;

namespace VideoGamesLibrary.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);

}
