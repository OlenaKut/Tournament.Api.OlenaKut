using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public class TournamentCreateDto : TournamentForManipulationDto
    {
        public IEnumerable<GameDto>? Games { get; set; }

    }
}
