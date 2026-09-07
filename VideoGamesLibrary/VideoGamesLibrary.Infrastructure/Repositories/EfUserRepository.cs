using Microsoft.EntityFrameworkCore;
using VideoGamesLibrary.Domain.Entities;
using VideoGamesLibrary.Domain.Repositories;
using VideoGamesLibrary.Infrastructure.Data;

namespace VideoGamesLibrary.Infrastructure.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly VideoGamesLibraryDbContext _context;

        public EfUserRepository(VideoGamesLibraryDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
