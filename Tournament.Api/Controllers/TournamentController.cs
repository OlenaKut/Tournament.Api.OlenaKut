using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;
using Tournament.Core.Entities;
using Tournament.Core.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Tournament.Core.Repositories;

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
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails(bool includeGames)
        {
            //return await _context.TournamentDetails.Include(g => g.Games).ToListAsync();
            var tournaments = includeGames ? _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(true))
                : _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(false));

            return Ok(tournaments);
        }

        // GET: api/Tournament/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id)
        {
            TournamentDetails? tournamentDetails = await _unitOfWork.TournamentRepository.GetAsync(id);

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
    }
}
