using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GameDto>> GetGamesAsync(int tournamentId)
        {
            return _mapper.Map<IEnumerable<GameDto>>(await _unitOfWork.GameRepository.GetGamesAsync(tournamentId));
        }

        public async Task<GameDto> GetGameAsync(int id)
        {
            Game? game = await _unitOfWork.GameRepository.GetGameAsync(id);
            return _mapper.Map<GameDto>(game);
        }

        public async Task<GameDto> GetGameByTitleAsync(int tournamentId, string title)
        {
            Game ? game = await _unitOfWork.GameRepository.GetGameByTitleAsync(tournamentId, title);
            return _mapper.Map<GameDto>(game);
        }

        public async Task<bool> AnyGameAsync(int id)
        {
            return await _unitOfWork.GameRepository.AnyGameAsync(id);
        }

        public async Task<GameDto> GreateGameAsync(int tournamentId, GameCreateDto dto)
        {
            var game = _mapper.Map<Game>(dto);
            game.TournamentDetailsId = tournamentId;
            
            _unitOfWork.GameRepository.AddGame(game);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<GameDto>(game); 

        }
        public async Task<bool> UpdateGameAsync(int id, GameUpdateDto dto)
        {
            var game = await _unitOfWork.GameRepository.GetGameAsync(id);
            if (game != null) return false;

            _mapper.Map(dto, game);
            
            _unitOfWork.GameRepository.UpdateGame(game);
            await _unitOfWork.CompleteAsync();
            return true;

        }
      
        public async Task<bool> DeleteGameAsync(int id)
        {
            var game = await _unitOfWork.GameRepository.GetGameAsync(id);
            if (game != null) return false;

            _unitOfWork.GameRepository.RemoveGame(game);
            await _unitOfWork.CompleteAsync();
            return true;
        }


    }
}
 