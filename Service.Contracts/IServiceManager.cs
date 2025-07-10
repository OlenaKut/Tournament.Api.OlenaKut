using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;

namespace Services.Contracts
{
    public interface IServiceManager
    {
        ITournamentService TournamentService { get; }
        IGameService GameService { get; }

        IAuthService AuthService { get; }

    }
}
