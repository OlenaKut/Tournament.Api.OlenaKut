using AutoMapper;
using AutoMapper.QueryableExtensions;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Migrations;

namespace Tournament.Api.Controllers
{
    [Route("api/Tournament")]
    [ApiController]
    [Produces("application/json")]
    public class TournamentController : ControllerBase
    {
        //private readonly TournamentApiContext _context;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TournamentController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            //_context = context;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // GET: api/Tournament
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails(bool includeGames, string? sortBy = null)
        {
            //return await _context.TournamentDetails.Include(g => g.Games).ToListAsync();
            var tournaments = includeGames ? _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(true))
                : _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(false)); 

            tournaments = sortBy?.ToLower() switch
            {
                "title" => tournaments.OrderBy(t => t.Title),
                "start" => tournaments.OrderBy(t => t.StartGame),
                "finish" => tournaments.OrderBy(t => t.EndDate),
                _ => tournaments
            };


            return Ok(tournaments);
        }

       // GET filtering and searching
       [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetFilteredAsync(string? title, string? searchQuery)
        {
            var tournaments = await _unitOfWork.TournamentRepository.GetFilteredAsync(title, searchQuery);

            if (tournaments == null || !tournaments.Any())
            {
                return NotFound("Tournament(s) not found");
            }

            var dto = _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
            return Ok(dto);
        }

        // GET: api/Tournament/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id, bool includeGames)
        {
            TournamentDetails? tournamentDetails = await _unitOfWork.TournamentRepository.GetAsyncWithGames(id, includeGames);

            if (tournamentDetails == null)
            {
                return NotFound("Tournament not found");
            }

            var dto = _mapper.Map<TournamentDto>(tournamentDetails);

            return dto;
        }

        // PUT: api/Tournament/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var existingTournament = await _unitOfWork.TournamentRepository.GetAsync(id);

            if (existingTournament == null)
            {
                return NotFound("Tournament does not exist");
            }

            _mapper.Map(dto, existingTournament);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        // POST: api/Tournament
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDto>> PostTournamentDetails(TournamentCreateDto dto)
        {
            var tournament = _mapper.Map<TournamentDetails>(dto);
            _unitOfWork.TournamentRepository.Add(tournament);
            await _unitOfWork.CompleteAsync();

            var createdTournament = _mapper.Map<TournamentDto>(tournament);

            return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournament.Id}, createdTournament);
        }

        // DELETE: api/Tournament/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            var existingTournament = await _unitOfWork.TournamentRepository.GetTournamentWithGamesAsync(id);

            if (existingTournament == null)
            {
                return NotFound("Tournament not found");
            }

            if(existingTournament.Games != null && existingTournament.Games.Any())
            {
                foreach (var game in existingTournament.Games.ToList())
                {
                    _unitOfWork.GameRepository.RemoveGame(game);
                }
            }


            _unitOfWork.TournamentRepository.Remove(existingTournament);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        //PATCH: api/Tournament/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchTournament(int id, JsonPatchDocument<TournamentUpdateDto> patchDocument)
        {
            if (patchDocument == null) return BadRequest("No patchdocument");

            var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
            if (tournament == null)
                return NotFound("Tournament not found");

            var dto = _mapper.Map<TournamentUpdateDto>(tournament);

            patchDocument.ApplyTo(dto, ModelState);
            if (!TryValidateModel(dto))
                return UnprocessableEntity(ModelState);

            _mapper.Map(dto, tournament);
            await _unitOfWork.CompleteAsync();

            return NoContent();

        }

    }
}
