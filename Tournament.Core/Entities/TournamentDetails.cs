using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Entities
{
    public class TournamentDetails
    {
    
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        [MaxLength(60, ErrorMessage = "Maximum length for the Title is 60 characters.")]
        public string? Title { get; set; }
        public DateTime StartGame { get; set; }
        public DateTime EndGame { get; set; }
        public List<Game>? Games { get; set; } = null!;

    }
}
