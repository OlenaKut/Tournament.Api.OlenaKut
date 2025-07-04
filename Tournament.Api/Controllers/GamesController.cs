﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Migrations;
using Tournament.Data.Repositories;

namespace Tournament.Api.Controllers
{
    [Route("api/tournament/{tournamentId}/Games")]
    [ApiController]
    [Produces("application/json")]
    public class GamesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GamesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames(int tournamentId, string? sortBy = null)
        {
            //return await _context.Game.ToListAsync();
            var tournamenExist = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);

            if (!tournamenExist)
            { 
                return NotFound("Tournament not found"); 
            }

            var games = await _unitOfWork.GameRepository.GetGamesAsync(tournamentId);

            games = sortBy?.ToLower() switch
            {
                "title" => games.OrderBy(t => t.Title),
                "start" => games.OrderBy(t => t.Time),
                _ => games
            };

            var gamesDto = _mapper.Map<IEnumerable<GameDto>>(games);

            return Ok(gamesDto);
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetGame(int tournamentId, int id)
        {
            var game = await _unitOfWork.GameRepository.GetGameAsync(id);

            if (game == null || game.TournamentDetailsId != tournamentId)
            {
                return NotFound();
            }

            return game;
        }

        //GET by title
        [HttpGet("by-title")]
        public async Task<ActionResult<GameDto>> GetGameByTitle(int tournamentId, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Title must be provided.");
            }

            var tournamentExists = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);
            if (!tournamentExists)
            {
                return NotFound("Tournament not found.");
            }

            var game = await _unitOfWork.GameRepository.GetGameByTitleAsync(tournamentId, title);

            if (game == null)
            {
                return NotFound($"No game with title '{title}' found in this tournament.");
            }

            var dto = _mapper.Map<GameDto>(game);
            return Ok(dto);
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var existingGame = await _unitOfWork.GameRepository.GetGameAsync(id);
            if (existingGame == null)
            {
                return NotFound("Game does not exist");
            }

            _mapper.Map(dto, existingGame);
            await _unitOfWork.CompleteAsync();
 
            return NoContent();
        }

        // POST: api/Games
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(int tournamentId, GameCreateDto dto)
        {
            var tournamentExists = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);
            if (!tournamentExists)
            {
                return NotFound("Tournament not found.");
            }

            var game = _mapper.Map<Game>(dto);
            game.TournamentDetailsId = tournamentId;

            _unitOfWork.GameRepository.AddGame(game);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(
                nameof(GetGame),
                new { tournamentId = game.TournamentDetailsId, id = game.Id },
                _mapper.Map<GameDto>(game)
            );
        }

        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _unitOfWork.GameRepository.GetGameAsync(id);
            if (game == null)
            {
                return NotFound("Game not found");
            }

            _unitOfWork.GameRepository.RemoveGame(game);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }


        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchGame(int id,  int tournamentId, JsonPatchDocument<GameUpdateDto> patchDocument)
        {
            if (patchDocument == null) 
                return BadRequest("No patchdocument");

            var tournamentExists = await _unitOfWork.TournamentRepository.TournamentExistAsync(tournamentId);

            if (!tournamentExists) 
                return NotFound("Tournament does not exist");

            var gameToPatch = await _unitOfWork.GameRepository.GetGameAsync(id);

            if (gameToPatch == null || gameToPatch.TournamentDetailsId != tournamentId)
                return NotFound("Game does not exist in this tournament");

            var dto = _mapper.Map<GameUpdateDto>(gameToPatch);

            patchDocument.ApplyTo(dto, ModelState);
            if (!TryValidateModel(dto))
                return UnprocessableEntity(ModelState);

            _mapper.Map(dto, gameToPatch);
            await _unitOfWork.CompleteAsync();

            return NoContent();

        }
    }
}
