using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Responses;

namespace Tournament.Presentation.Controllers
{
    //Add Api version to the route
    //[Route("api/v{version:apiVersion}/tournament/{tournamentId}/Games")]
    [Route("api/tournament/{tournamentId}/Games")]
    [ApiController]
    [Produces("application/json")]
    //[ApiVersion(2)]
    //[ApiVersion(1)]
    //[ApiVersion(0.1, Deprecated = true)]
    public class GamesController : ApiControllerBase
    {
        private readonly IServiceManager _serviceManager;


        public GamesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames(int tournamentId, string? sortBy = null)
        {
            //return await _context.Game.ToListAsync();
            //var tournamenExist = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);

            //if (!tournamenExist)
            //{ 
            //    return NotFound("Tournament not found"); 
            //}

            //var games = await _unitOfWork.GameRepository.GetGamesAsync(tournamentId);

            //games = sortBy?.ToLower() switch
            //{
            //    "title" => games.OrderBy(t => t.Title),
            //    "start" => games.OrderBy(t => t.Time),
            //    _ => games
            //};

            //var gamesDto = _mapper.Map<IEnumerable<GameDto>>(games);

            //return Ok(gamesDto);
            //var tournamenExist = await _serviceManager.TournamentService.TournamentExistAsync(tournamentId);
            //if (!tournamenExist) return NotFound("Tournament not found");

            //var games = await _serviceManager.GameService.GetGamesAsync(tournamentId);

            //games = sortBy?.ToLower() switch
            //{
            //    "title" => games.OrderBy(t => t.Title),
            //    "start" => games.OrderBy(t => t.Time),
            //    _ => games
            //};
            //return Ok(games);

            var response = await _serviceManager.GameService.GetGamesAsync(tournamentId);

            if (response is not ApiOkResponse<IEnumerable<GameDto>> okResponse)
                return ProcessError(response);

            var games = okResponse.Result;

            games = sortBy?.ToLower() switch
            {
                "title" => games.OrderBy(g => g.Title),
                "start" => games.OrderBy(g => g.Time),
                _ => games
            };

            return Ok(games);   
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGameAsync(int tournamentId, int id)
        {
            //var game = await _unitOfWork.GameRepository.GetGameAsync(id);

            //if (game == null || game.TournamentDetailsId != tournamentId)
            //{
            //    return NotFound();
            //}

            //return game;
            var game = await _serviceManager.GameService.GetGameAsync(id);

            if (game == null || game.TournamentDetailsId != tournamentId) return NotFound("Tournament not found");

            return Ok(game);
        }

        //GET by title
        [HttpGet("by-title")]
        public async Task<ActionResult<GameDto>> GetGameByTitle(int tournamentId, string title)
        {
            //if (string.IsNullOrWhiteSpace(title))
            //{
            //    return BadRequest("Title must be provided.");
            //}

            //var tournamentExists = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);
            //if (!tournamentExists)
            //{
            //    return NotFound("Tournament not found.");
            //}

            //var game = await _unitOfWork.GameRepository.GetGameByTitleAsync(tournamentId, title);

            //if (game == null)
            //{
            //    return NotFound($"No game with title '{title}' found in this tournament.");
            //}

            //var dto = _mapper.Map<GameDto>(game);
            //return Ok(dto);

            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title must be provided.");
            }
            var tournamentExists = await _serviceManager.TournamentService.TournamentExistAsync(tournamentId);
            if (!tournamentExists) return NotFound("Tournament not found");

            var game = await _serviceManager.GameService.GetGameByTitleAsync(tournamentId, title);

            if (game == null) return NotFound($"No game with title '{title}' found in this tournament.");

            return Ok(game);
        }


        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameUpdateDto dto)
        {
            //if (id != dto.Id)
            //{
            //    return BadRequest();
            //}

            //var existingGame = await _unitOfWork.GameRepository.GetGameAsync(id);
            //if (existingGame == null)
            //{
            //    return NotFound("Game does not exist");
            //}

            //_mapper.Map(dto, existingGame);
            //await _unitOfWork.CompleteAsync();

            //return NoContent();
            if (id != dto.Id) return BadRequest("ID mismatch");

            var updatedGame = await _serviceManager.GameService.UpdateGameAsync(id, dto);
            if (!updatedGame) return NotFound("Game not found");
            return NoContent();

        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(int tournamentId, GameCreateDto dto)
        {
            //var tournamentExists = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);
            //if (!tournamentExists)
            //{
            //    return NotFound("Tournament not found.");
            //}

            //var game = _mapper.Map<Game>(dto);
            //game.TournamentDetailsId = tournamentId;

            //_unitOfWork.GameRepository.AddGame(game);
            //await _unitOfWork.CompleteAsync();

            //return CreatedAtAction(
            //    nameof(GetGame),
            //    new { tournamentId = game.TournamentDetailsId, id = game.Id },
            //    _mapper.Map<GameDto>(game)
            //);
            var tournamentExists = await _serviceManager.TournamentService.TournamentExistAsync(tournamentId);
            if (!tournamentExists) return NotFound("Tournament not found.");

            var createdGame = await _serviceManager.GameService.GreateGameAsync(tournamentId, dto);
            return CreatedAtAction(nameof(GetGameAsync), new
            {
                tournamentId = tournamentId,
                id = createdGame.Id
            }, createdGame);

        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            //var game = await _unitOfWork.GameRepository.GetGameAsync(id);
            //if (game == null)
            //{
            //    return NotFound("Game not found");
            //}

            //_unitOfWork.GameRepository.RemoveGame(game);
            //await _unitOfWork.CompleteAsync();

            //return NoContent();

            var deleteGame = await _serviceManager.GameService.DeleteGameAsync(id);
            if (!deleteGame) return NotFound("Game not found");
            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchGame(int id,  int tournamentId, JsonPatchDocument<GameUpdateDto> patchDocument)
        {
            //if (patchDocument == null) 
            //    return BadRequest("No patchdocument");

            //var tournamentExists = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);

            //if (!tournamentExists) 
            //    return NotFound("Tournament does not exist");

            //var gameToPatch = await _unitOfWork.GameRepository.GetGameAsync(id);

            //if (gameToPatch == null || gameToPatch.TournamentDetailsId != tournamentId)
            //    return NotFound("Game does not exist in this tournament");

            //var dto = _mapper.Map<GameUpdateDto>(gameToPatch);

            //patchDocument.ApplyTo(dto, ModelState);
            //if (!TryValidateModel(dto))
            //    return UnprocessableEntity(ModelState);

            //_mapper.Map(dto, gameToPatch);
            //await _unitOfWork.CompleteAsync();

            //return NoContent();

            if (patchDocument == null) return BadRequest("No patchdocument");

            var tournamentExists = await _serviceManager.TournamentService.TournamentExistAsync(tournamentId);
            if (!tournamentExists) return NotFound("Tournament not found");

            var game = await _serviceManager.GameService.GetGameAsync(id);
            if (game == null || game.TournamentDetailsId != tournamentId) return NotFound("Game not found in this tournament.");

            var gameUpdateDto = new GameUpdateDto
            {
                Id = game.Id,
                Title = game.Title,
                Time = game.Time,
            };

            patchDocument.ApplyTo(gameUpdateDto, ModelState);
            if (!TryValidateModel(gameUpdateDto))
                return UnprocessableEntity(ModelState);

            var updated = await _serviceManager.GameService.UpdateGameAsync(id, gameUpdateDto);
            return updated ? NoContent() : NotFound("Game not found");

        }
    }
}
