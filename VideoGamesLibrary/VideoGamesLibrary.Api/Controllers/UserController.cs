using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VideoGamesLibrary.Application.Dtos;
using VideoGamesLibrary.Application.Interfaces;

namespace VideoGamesLibrary.Api.Controllers;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userService.Login(request);

        if (!result.Success || string.IsNullOrEmpty(result.Token))
        {
            return Unauthorized(new { error = result.ErrorMessage });
        }

        return Ok(new { token = result.Token });
    }
}
