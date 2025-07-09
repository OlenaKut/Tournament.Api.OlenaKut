using Bogus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private static UserManager<ApplicationUser> userManager;
        private static RoleManager<IdentityRole> roleManager;
        private static IConfiguration configuration;

        private const string employeeRole = "Employee";
        private const string adminRole = "Admin";
        public static async Task SeedDataAsync(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var db = serviceProvider.GetRequiredService<TournamentApiContext>();

                await db.Database.MigrateAsync();



                userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                configuration = serviceProvider.GetRequiredService<IConfiguration>();

                try
                {
                    await CreateRoleAsync(new[] { adminRole, employeeRole });
                    await CreateConfiguratedUserAsync();

                    if (await db.TournamentDetails.AnyAsync())
                    {
                        return; // // Database has been seeded
                    }

                    // Create demo user for sample tournaments
                    var demoUserEmail = "demo@user.com";
                    var demoUser = await userManager.FindByEmailAsync(demoUserEmail);
                    if (demoUser == null)
                    {
                        demoUser = new ApplicationUser
                        {
                            UserName = demoUserEmail,
                            Email = demoUserEmail,
                            FirstName = "Demo",
                            LastName = "User"
                        };

                        var result = await userManager.CreateAsync(demoUser, "Pass123!");
                        if (!result.Succeeded)
                            throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));

                        await userManager.AddToRoleAsync(demoUser, employeeRole);
                    }

                    var tournaments = GenerateTournaments(5, demoUser.Id);
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

        private static async Task CreateRoleAsync(string[] roleNames)
        {
            foreach(var roleName in roleNames)
            {
                if(await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }
            }
        }

        private static async Task CreateConfiguratedUserAsync()
        {
            var adminEmail = configuration["AdminUser:Email"];
            var adminPassword = configuration["AdminUser:Password"];
            var userEmail = configuration["DefaultUser:Email"];
            var userPassword = configuration["DefaultUser:Password"];

            if (!string.IsNullOrWhiteSpace(adminEmail) && await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, adminRole);
            }


            if (!string.IsNullOrWhiteSpace(userEmail) && await userManager.FindByEmailAsync(userEmail) is null)
            {
                var user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "Default",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, employeeRole);
            }
        }

        private static List<TournamentDetails> GenerateTournaments(int count, string userId)
        {
            var faker = new Faker<TournamentDetails>("sv").Rules((f, t) =>
            {
                t.Title = f.Lorem.Word().ToUpper() + " Championship";
                t.StartGame = f.Date.Future();
                t.EndGame = t.StartGame.AddMonths(3);
                t.Games = new List<Game>();
                t.ApplicationUserId = userId;
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
