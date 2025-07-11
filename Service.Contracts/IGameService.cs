using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Responses;

namespace Services.Contracts
{
    public interface IGameService
    {
        Task<ApiBaseResponse> GetGamesAsync(int tournamentId);
        Task<GameDto> GetGameAsync(int id);
        Task<GameDto> GetGameByTitleAsync(int tournamentId, string title);
        Task<bool> AnyGameAsync(int id);
        Task<GameDto> GreateGameAsync(int tournamentId, GameCreateDto dto);
        Task<bool> UpdateGameAsync(int id, GameUpdateDto dto);
        Task<bool> DeleteGameAsync(int id);


    }
}
