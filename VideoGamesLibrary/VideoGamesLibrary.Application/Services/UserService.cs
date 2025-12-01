using VideoGamesLibrary.Application.Dtos;
using VideoGamesLibrary.Application.Interfaces;
using VideoGamesLibrary.Domain.Repositories;

namespace VideoGamesLibrary.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;

        public UserService(IUserRepository userRepository, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<LoginResultDto> Login(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return new LoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password."
                };
            }

            // Démo : comparaison directe.
            // À expliquer : en vrai, comparer un mot de passe haché et salé.
            if (user.Password != request.Password)
            {
                return new LoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Invalid username or password."
                };
            }

            var token = _tokenGenerator.GenerateToken(user);

            return new LoginResultDto
            {
                Success = true,
                Token = token
            };
        }
    }
}
