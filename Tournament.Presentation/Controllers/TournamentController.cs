using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.Options;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;


namespace Tournament.Presentation.Controllers
{
    
    [ApiController]
    [Route("api/Tournament")]
    [Produces("application/json")]
    [ApiVersion(1)]
    public class TournamentController : ControllerBase
    {
        //private readonly TournamentApiContext _context;
        //private readonly IMapper _mapper;
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceManager _serviceManager;
        private readonly UserManager<ApplicationUser> _userManager;

        const int maxTournamentPageSize = 15;

        public TournamentController(IServiceManager serviceManager, UserManager<ApplicationUser> userManager)
        {
            //_context = context;
            //_mapper = mapper;
            //_unitOfWork = unitOfWork;
            _serviceManager = serviceManager;
            _userManager = userManager;
        }

        //// GET: api/Tournament
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails(bool includeGames, string? sortBy = null)
        //{
        //    //return await _context.TournamentDetails.Include(g => g.Games).ToListAsync();
        //    var tournaments = includeGames ? _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(true))
        //        : _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(false)); 

        //    tournaments = sortBy?.ToLower() switch
        //    {
        //        "title" => tournaments.OrderBy(t => t.Title),
        //        "start" => tournaments.OrderBy(t => t.StartGame),
        //        "finish" => tournaments.OrderBy(t => t.EndDate),
        //        _ => tournaments
        //    };
        //    return Ok(tournaments);
        //}

        // GET: api/Tournament
        [HttpGet]
        //[Authorize]
        //[Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails(bool includeGames)
        {
            //var auth = User.Identity.IsAuthenticated;
            //var userName = _userManager.GetUserName(User);
            //var user = await _userManager.GetUserAsync(User);
           
            var tournamentDto = await _serviceManager.TournamentService.GetAllAsync(includeGames);

            //tournaments = sortBy?.ToLower() switch
            //{
            //    "title" => tournaments.OrderBy(t => t.Title),
            //    "start" => tournaments.OrderBy(t => t.StartGame),
            //    "finish" => tournaments.OrderBy(t => t.EndDate),
            //    _ => tournaments
            //};
            return Ok(tournamentDto);
        }


        // GET filtering and searching
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetFilteredAsync(string? title, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            //if (pageSize > maxTournamentPageSize)
            //{
            //    pageSize = maxTournamentPageSize;
            //}

            //var (tournaments, paginationMetadata) = await _unitOfWork.TournamentRepository
            //    .GetFilteredAsync(title, searchQuery, pageNumber, pageSize);

            //Response.Headers.Append("X-Pagination",
            //    JsonSerializer.Serialize(paginationMetadata));

            //if (tournaments == null || !tournaments.Any())
            //{
            //    return NotFound("Tournament(s) not found");
            //}

            //var dto = _mapper.Map<IEnumerable<TournamentDto>>(tournaments);
            //return Ok(dto);
            if (pageSize > maxTournamentPageSize)
                pageSize = maxTournamentPageSize;

            var (tournaments, paginationMetadata) = await _serviceManager.TournamentService.GetFilteredAsync(title, searchQuery, pageNumber, pageSize);

            if (tournaments == null || !tournaments.Any())
            {
                return NotFound("Tournament(s) not found");
            }

            Response.Headers.Append("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(tournaments);

        }

        //[Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        //[Authorize(Policy = "AdminPolicy")]
        // GET: api/Tournament/5

        //XML comments
        /// <summary>
        /// Get the tournament by Id
        /// </summary>
        /// <param name="id">The Id of the tournament</param>
        /// <param name="includeGames">Includes or now the games</param>
        /// <returns>A tournament with or without games</returns>
        /// 
        /// <response code="200">Returns the requested tournament</response>
        [HttpGet("{id:int}")]
        //Describing response types
        
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id, bool includeGames)
        {
            //TournamentDetails? tournamentDetails = await _unitOfWork.TournamentRepository.GetAsyncWithGames(id, includeGames);

            //if (tournamentDetails == null)
            //{
            //    return NotFound("Tournament not found");
            //}

            //var dto = _mapper.Map<TournamentDto>(tournamentDetails);

            //return dto;
            TournamentDto dto = await _serviceManager.TournamentService.GetAsyncWithGames(id, includeGames);

            if (dto == null) return NotFound("Tournament not found");

            return Ok(dto);
        }

        // PUT: api/Tournament/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentUpdateDto dto)
        {
            //if (id != dto.Id)
            //{
            //    return BadRequest();
            //}

            //var existingTournament = await _unitOfWork.TournamentRepository.GetAsync(id);

            //if (existingTournament == null)
            //{
            //    return NotFound("Tournament does not exist");
            //}

            //_mapper.Map(dto, existingTournament);
            //await _unitOfWork.CompleteAsync();

            //return NoContent();

            if (id != dto.Id) return BadRequest("ID mismatch");

            var success = await _serviceManager.TournamentService.UpdateAsync(id, dto);

            return success ? NoContent() : NotFound("Tournament not found");


        }

        // POST: api/Tournament
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDto>> PostTournamentDetails(TournamentCreateDto dto)
        {
            //var tournament = _mapper.Map<TournamentDetails>(dto);
            //_unitOfWork.TournamentRepository.Add(tournament);
            //await _unitOfWork.CompleteAsync();

            //var createdTournament = _mapper.Map<TournamentDto>(tournament);

            //return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournament.Id}, createdTournament);
            var createdTournament = await _serviceManager.TournamentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = createdTournament.Id}, createdTournament);

        }

        // DELETE: api/Tournament/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            //var existingTournament = await _unitOfWork.TournamentRepository.GetTournamentWithGamesAsync(id);

            //if (existingTournament == null)
            //{
            //    return NotFound("Tournament not found");
            //}

            //if(existingTournament.Games != null && existingTournament.Games.Any())
            //{
            //    foreach (var game in existingTournament.Games.ToList())
            //    {
            //        _unitOfWork.GameRepository.RemoveGame(game);
            //    }
            //}


            //_unitOfWork.TournamentRepository.Remove(existingTournament);
            //await _unitOfWork.CompleteAsync();

            //return NoContent();
            var success = await _serviceManager.TournamentService.DeleteAsync(id);
            return success ? NoContent() : NotFound("Tournament not found");


        }

        //PATCH: api/Tournament/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchTournament(int id, JsonPatchDocument<TournamentUpdateDto> patchDocument)
        {
            //if (patchDocument == null) return BadRequest("No patchdocument");

            //var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
            //if (tournament == null)
            //    return NotFound("Tournament not found");

            //var dto = _mapper.Map<TournamentUpdateDto>(tournament);

            //patchDocument.ApplyTo(dto, ModelState);
            //if (!TryValidateModel(dto))
            //    return UnprocessableEntity(ModelState);

            //_mapper.Map(dto, tournament);
            //await _unitOfWork.CompleteAsync();

            //return NoContent();

            if (patchDocument == null) return BadRequest("No patchdocument");
            var tournament = await _serviceManager.TournamentService.GetAsyncWithGames(id, false);

            if (tournament == null) NotFound("Tournament not found");

            var updatedDto = new TournamentUpdateDto
            {
                Id = tournament.Id,
                Title = tournament.Title,
                StartGame = tournament.StartGame
            };

            patchDocument.ApplyTo(updatedDto, ModelState);
            if (!TryValidateModel(updatedDto))
                return UnprocessableEntity(ModelState);

            var updated = await _serviceManager.TournamentService.UpdateAsync(id, updatedDto);
            return updated ? NoContent() : NotFound("Tournament not found");

        }

    }
}
