using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGamesLibrary.Application.Dtos;
using VideoGamesLibrary.Application.Interfaces;

namespace VideoGamesLibrary.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    // GET: api/games
    [HttpGet]
    public async Task<ActionResult<List<GameDto>>> GetAll()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    // GET: api/games/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameDto>> GetById(int id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    // POST: api/games
    [HttpPost]
    public async Task<ActionResult<GameDto>> Create([FromBody] CreateGameDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = await _gameService.AddAsync(dto);

        // Retourne 201 Created avec l’URL du nouvel objet
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/games/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGameDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _gameService.UpdateAsync(id, dto);
        if (!success)
        {
            return NotFound();
        }

        return NoContent(); // 204
    }

    // DELETE: api/games/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _gameService.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
