using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Data.Migrations;

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
                    db.TournamentDetails.AddRange(tournaments);
                    await db.SaveChangesAsync();

                    foreach (var tournament in tournaments)
                    {
                        var games = GenerateNonOverlappingGames(
                            tournament.StartGame,
                            tournament.EndGame,
                            count: 3,
                            tournament.Id,
                            tournament
                            );
                        //tournament.Games?.AddRange(games);
                        db.Game.AddRange(games);
                    }

                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error seeding data: " + ex.Message, ex);
                }

            }
        }

        private static List<TournamentDetails> GenerateTournaments(int count)
        {
            var faker = new Faker<TournamentDetails>("sv").Rules((f, t) =>
            {
                t.Title = f.Lorem.Word().ToUpper() + " Championship";
                t.StartGame = f.Date.Future();
                t.EndGame = t.StartGame.AddMonths(3);
                t.Games = new List<Game>();
            });

            return faker.Generate(count);
        }

        private static List<Game> GenerateNonOverlappingGames(DateTime start, DateTime end, int count, int tournamentId, TournamentDetails tournament)
        {
            var games = new List<Game>();
            var totalDays = (end - start).Days;

            if (totalDays <= 0 || count > totalDays)
            {
                throw new ArgumentException("Tournament period is too short for the number of non-overlapping games.");

            }
            var faker = new Faker();
            var usedDates = new List<DateTime>();

            while (games.Count < count)
            {
                var randomDay = faker.Random.Int(0, totalDays -1);
                var gameDate = start.Date.AddDays(randomDay);

                if (usedDates.Contains(gameDate))
                    continue;
                usedDates.Add(gameDate);

                var game = new Game
                {
                    Title = faker.Lorem.Word().ToUpper(),
                    Time = gameDate,
                    TournamentDetailsId = tournamentId,
                    TournamentDetails = tournament
                };
                games.Add(game);
            }
            return games;
        }

        //private static ICollection<Game> GenerateGames(int count, int tournamentId)
        //{
        //    var faker = new Faker<Game>("sv")
        //        g.Title = f.Lorem.Word().ToUpper();
        //        g.Time = f.Date.Future();.Rules((f, g) =>
        //    {
        //        g.TournamentId = tournamentId;
        //    });
        //    return faker.Generate(count);
        //}
    }
}
