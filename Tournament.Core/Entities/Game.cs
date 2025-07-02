using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Tournament.Core.Entities
{
    public class Game
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Title is 60 characters.")]
        public string? Title { get; set; }
        public DateTime Time { get; set; }
        public int TournamentId { get; set; }
        public TournamentDetails? TournamentDetails { get; set; } 
    }
}
