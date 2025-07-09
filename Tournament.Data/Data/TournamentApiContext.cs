using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Data.Data
{
    public class TournamentApiContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public TournamentApiContext(DbContextOptions<TournamentApiContext> options)
             : base(options)
        {
        }

        ////Used only by EF Tools at design-time to scaffold migrations
        //public TournamentApiContext()
        //    : base(new DbContextOptionsBuilder<TournamentApiContext>()
        //        .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TempDb;Trusted_Connection=True;MultipleActiveResultSets=true")
        //        .Options)
        //{
        //}

        public DbSet<TournamentDetails> TournamentDetails { get; set; } = default!;
        public DbSet<Game> Game { get; set; } = default!;
    }
}
