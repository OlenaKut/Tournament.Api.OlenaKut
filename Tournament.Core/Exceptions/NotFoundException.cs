using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        public string Title { get; set; }
        protected NotFoundException(string message, string title = "Not Found") : base(message)  
        {
            Title = title;
        }
    }

    public class TournamentNotFoundResponce : NotFoundException
    {
        public TournamentNotFoundResponce(int id) : base($"The tournament with id {id} is not found")
        {

        }
    }

    public class GameNotFoundException : NotFoundException
    {
        public GameNotFoundException(int id) : base($"The game with id {id} is not found")
        {

        }
    }
}
