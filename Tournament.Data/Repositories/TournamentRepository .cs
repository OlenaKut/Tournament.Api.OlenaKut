using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Tournament.Services;


namespace Tournament.Data.Repositories
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly TournamentApiContext _context;
        public TournamentRepository(TournamentApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TournamentDetails>> GetAllAsync(bool includeGames)
        {
            return includeGames ? await _context.TournamentDetails.Include(g => g.Games).ToListAsync()
                : await _context.TournamentDetails.ToListAsync();
        }


        // Filtering by title
        //Searching via collection
        public async Task<(IEnumerable<TournamentDetails>, PaginationMetadata)> GetFilteredAsync(
            string? title, string? searchQuery, int pageNumber, int pageSize)
        {

            //Returns all tournaments if no filters and searching
            //if (string.IsNullOrEmpty(title)
            //    && string.IsNullOrWhiteSpace(searchQuery))
            //{
            //    return await GetAllAsync(true);
            //}

            //Collection to start with
            var collection = _context.TournamentDetails as IQueryable<TournamentDetails>;

            if (!string.IsNullOrWhiteSpace(title))
            {
                var trimmed = title.Trim();
                collection = collection.Where(t => t.Title.Contains(trimmed));
            }


            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => 
                a.Title.Contains(searchQuery) || 
                (a.StartGame != null && a.StartGame.ToString().Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
          
            var collectionToReturn = await collection.OrderBy(t => t.Title)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);

            //title = title.Trim();
            //return await _context.TournamentDetails
            //    .Where(t => t.Title == title)
            //    .OrderBy(t => t.Title)
            //    .ToListAsync();
        }

        public async Task<TournamentDetails?> GetAsyncWithGames(int id, bool includeGames)
        {
     
            return includeGames ? await _context.TournamentDetails.Include(g => g.Games).FirstOrDefaultAsync(t => t.Id == id)
               : await _context.TournamentDetails.FirstOrDefaultAsync(t => t.Id == id);

            //return await _context.TournamentDetails.FindAsync(id);
        }

        public async Task<TournamentDetails?> GetAsync(int id)
        {
          return await _context.TournamentDetails.FindAsync(id);
        }

        public async Task<bool> TournamentExistAsync(int id)
        {
            return await _context.TournamentDetails.AnyAsync(t => t.Id == id);
        }

        public void Add(TournamentDetails tournament)
        {
            _context.TournamentDetails.Add(tournament);
        }

        public void Update(TournamentDetails tournament)
        {
            _context.TournamentDetails.Update(tournament);
        }

        public void Remove(TournamentDetails tournament)
        {
            _context.TournamentDetails.Remove(tournament); 
        }

        public async Task<TournamentDetails?> GetTournamentWithGamesAsync(int id)
        {
            return await _context.TournamentDetails
                .Include(t => t.Games)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
