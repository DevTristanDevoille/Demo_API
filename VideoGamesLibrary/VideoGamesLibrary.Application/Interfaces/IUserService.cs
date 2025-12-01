using VideoGamesLibrary.Application.Dtos;

namespace VideoGamesLibrary.Application.Interfaces;

public interface IUserService
{
    Task<LoginResultDto> Login(LoginRequestDto loginRequestDto);
}
