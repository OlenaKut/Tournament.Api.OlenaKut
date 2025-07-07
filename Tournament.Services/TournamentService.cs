using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.Services
{
    public class TournamentService : ITournamentService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public TournamentService(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        
        }

        public async Task<IEnumerable<TournamentDto>> GetAllAsync(bool includeGames)
        {
            return _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(includeGames));
        }

        public async Task<TournamentDto> GetAsyncWithGames(int id, bool includeGames)
        {
            TournamentDetails ? tournament = await _unitOfWork.TournamentRepository.GetAsyncWithGames(id, includeGames);

            return _mapper.Map<TournamentDto>(tournament);
        }

        public async Task<bool> TournamentExistAsync(int id)
        {
            return await _unitOfWork.TournamentRepository.TournamentExistAsync(id);
        }

        public async Task<TournamentDto> CreateAsync(TournamentCreateDto dto)
        {
            var tournament = _mapper.Map<TournamentDetails>(dto);
            _unitOfWork.TournamentRepository.Add(tournament);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TournamentDto>(tournament);

        }

        public async Task<bool> UpdateAsync(int id, TournamentUpdateDto dto)
        {
            var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
            if (tournament == null) return false;

            _mapper.Map(dto, tournament);
            ////////!!!!!!!!!!!!!!!!!!
    
            await _unitOfWork.CompleteAsync();
            return true;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tournament = await _unitOfWork.TournamentRepository.GetTournamentWithGamesAsync(id);
            if (tournament == null) return false;

            if (tournament.Games != null)
            {
                foreach (var game in tournament.Games)
                {
                    _unitOfWork.GameRepository.RemoveGame(game);
                }
            }
            _unitOfWork.TournamentRepository.Remove(tournament);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<(IEnumerable<TournamentDto>, PaginationMetadata)> GetFilteredAsync(string? title, string? searchQuery, int pageNumber, int pageSize)
        {
            var (entities, metadata) = await _unitOfWork.TournamentRepository.GetFilteredAsync(title, searchQuery, pageNumber, pageSize);
            var dtos = _mapper.Map<IEnumerable<TournamentDto>>(entities);
            return (dtos, metadata);
        }


    }
}
