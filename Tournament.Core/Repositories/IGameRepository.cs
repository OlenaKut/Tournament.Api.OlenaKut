using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Core.Repositories
{
    public interface IGameRepository
    {
        Task<IEnumerable<Game>> GetGamesAsync(int tournamentId);
        Task<Game?> GetGameAsync(int id);
        Task<bool> AnyGameAsync(int id);
        void AddGame(Game game);
        void UpdateGame(Game game);
        void RemoveGame(Game game);

        //Task<IEnumerable<Game>> GetAllByTournamentIdAsync(int tournamentId);

    }
}
