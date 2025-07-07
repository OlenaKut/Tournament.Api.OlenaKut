using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;

namespace Services.Contracts
{
    public interface IGameService
    {
        Task<IEnumerable<GameDto>> GetGamesAsync(int tournamentId);
        Task<GameDto> GetGameAsync(int id);
        Task<GameDto> GetGameByTitleAsync(int tournamentId, string title);
        Task<bool> AnyGameAsync(int id);
        Task<GameDto> GreateGameAsync(int tournamentId, GameCreateDto dto);
        Task<bool> UpdateGameAsync(int id, GameUpdateDto dto);
        Task<bool> DeleteGameAsync(int id);


    }
}
