using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;

namespace Tournament.Data.Data
{
    public static class SeedData
    {
        public static async Task SeedDataAsync(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var db = serviceProvider.GetRequiredService<TournamentApiContext>();

                await db.Database.MigrateAsync();
                if (await db.TournamentDetails.AnyAsync())
                {
                    return; // // Database has been seeded
                }

                try
                {
                    var tournaments = GenerateTournaments(5);
                    db.AddRange(tournaments);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }

            }
        }

        private static List<TournamentDetails> GenerateTournaments(int nrOfTournaments)
        {
            var tournamentFaker = new Faker<TournamentDetails>("sv")
                .RuleFor(t => t.Title, f => f.Lorem.Word() + " Championship")
                .RuleFor(t => t.StartGame, f => f.Date.Future())
                .RuleFor(t => t.Games, (f, t) =>
                {
                    var gameFaker = new Faker<Game>("sv")
                        .RuleFor(g => g.Title, f2 => f2.Lorem.Word())
                        .RuleFor(g => g.Time, f2 => f2.Date.Between(t.StartGame, t.StartGame.AddMonths(3)))
                        .RuleFor(g => g.TournamentDetails, _ => t); // ✅ link back to parent tournament

                    return gameFaker.Generate(f.Random.Int(3, 6));
                });

            return tournamentFaker.Generate(nrOfTournaments);
        }
    }
}
