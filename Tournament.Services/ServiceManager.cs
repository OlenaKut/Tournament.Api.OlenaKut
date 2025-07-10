using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
    public class ServiceManager : IServiceManager
    {
       private readonly Lazy<ITournamentService> tournamentService;
       private readonly Lazy<IGameService> gameService;
       private readonly Lazy<IAuthService> authService;

        public ITournamentService TournamentService => tournamentService.Value;
        public IGameService GameService => gameService.Value;
        public IAuthService AuthService => authService.Value;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            ArgumentNullException.ThrowIfNull(nameof(unitOfWork));

            tournamentService = new Lazy<ITournamentService>(() => new TournamentService(unitOfWork, mapper));
            gameService = new Lazy<IGameService>(() => new GameService(unitOfWork, mapper));

            authService = new Lazy<IAuthService>(() => new AuthService(mapper, userManager, roleManager, config));
        }

        //public ServiceManager(Lazy<ITournamentService> tournamentService, Lazy<IGameService> gameService, Lazy<IAuthService> authService)
        //{
        //    this.tournamentService = tournamentService;
        //    this.gameService = gameService;
        //    this.authService = authService;

        //}
    }
}
