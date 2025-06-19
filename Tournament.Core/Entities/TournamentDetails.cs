using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities
{
    public class TournamentDetails
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime StartGame { get; set; }
        ICollection<Game> Games { get; set; }
    }
}
