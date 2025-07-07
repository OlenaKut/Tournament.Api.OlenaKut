using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record TournamentUpdateDto : TournamentForManipulationDto
    {
        public int Id { get; set; }
    }
}
