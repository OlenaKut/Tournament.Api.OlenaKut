using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tournament.Core.DTOs
{
    public record TournamentDto
    {
        /// <summary>
        ///     The Id of the tournament
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The Title of the tournament
        /// </summary>
        public string? Title { get; init; }

        /// <summary>
        /// The start of the tournament
        /// </summary>
        public DateTime StartGame { get; init; }

        /// <summary>
        /// The end of the tournament
        /// </summary>
        public DateTime EndDate => StartGame.AddMonths(3);

        /// <summary>
        /// The Games of the tournament
        /// </summary>
        public IEnumerable<GameDto>? Games { get; init; }
    }
}
