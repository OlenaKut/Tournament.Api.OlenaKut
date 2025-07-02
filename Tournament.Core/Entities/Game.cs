using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tournament.Core.Entities
{
    public class Game
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Title is 60 characters.")]
        public string? Title { get; set; }
        public DateTime Time { get; set; }
        public int TournamentDetailsId { get; set; }
        [ForeignKey("TournamentDetailsId")]
        public TournamentDetails? TournamentDetails { get; set; } 
    }
}
