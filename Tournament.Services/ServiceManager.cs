using AutoMapper;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;


namespace Tournament.Services
{
    public class ServiceManager : IServiceManager
    {
       private readonly Lazy<ITournamentService> tournamentService;
       private readonly Lazy<IGameService> gameService;

        public ITournamentService TournamentService => tournamentService.Value;
        public IGameService GameService => gameService.Value;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(nameof(unitOfWork));

            tournamentService = new Lazy<ITournamentService>(() => new TournamentService(unitOfWork, mapper));
            gameService = new Lazy<IGameService>(() => new GameService(unitOfWork, mapper));
        }

    }
}
