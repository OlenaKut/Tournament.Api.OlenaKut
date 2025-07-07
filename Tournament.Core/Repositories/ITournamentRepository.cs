using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Services;
namespace Tournament.Core.Repositories
{
    public interface ITournamentRepository
    {
        Task<IEnumerable<TournamentDetails>> GetAllAsync(bool includeGames);
        Task<(IEnumerable<TournamentDetails>, PaginationMetadata)> GetFilteredAsync(
            string? title,string? searchQuery, int pageNumber, int pageSize);
        Task<TournamentDetails?> GetAsync(int id);
        Task<TournamentDetails?> GetAsyncWithGames(int id, bool includeGames);
        Task<bool> TournamentExistAsync(int id);
        void Add(TournamentDetails tournament);
        void Update(TournamentDetails tournament);
        void Remove(TournamentDetails tournament);
        Task<TournamentDetails?> GetTournamentWithGamesAsync(int id);


    }
}
