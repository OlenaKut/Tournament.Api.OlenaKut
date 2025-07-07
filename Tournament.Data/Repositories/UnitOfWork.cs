using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TournamentApiContext _context;
        private readonly Lazy<ITournamentRepository> tournamentRepository;
        private readonly Lazy<IGameRepository> gameRepository;
        //public ITournamentRepository TournamentRepository { get; }
        //public IGameRepository GameRepository { get; }

        public ITournamentRepository TournamentRepository  => tournamentRepository.Value;
        public IGameRepository GameRepository => gameRepository.Value;

        //public UnitOfWork(TournamentApiContext context)
        //{
        //    _context = context;
        //    TournamentRepository = new TournamentRepository(context);
        //    GameRepository = new GameRepository(context);
        //}

        public UnitOfWork(TournamentApiContext context, Lazy<IGameRepository> gamerepository, Lazy<ITournamentRepository> tournamentrepository)
        {
            _context = context;
            tournamentRepository = tournamentrepository;
            gameRepository = gamerepository;

        }

        public async Task CompleteAsync() => await _context.SaveChangesAsync();
    }
}
