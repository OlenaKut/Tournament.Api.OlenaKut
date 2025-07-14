using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Tournament.Api.Extensions;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Repositories;
using Tournament.Presentation;
using Tournament.Services;

namespace Tournament.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Enable console logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddDbContext<TournamentApiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TournamentApiContext") ?? throw new InvalidOperationException("Connection string 'TournamentApiContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers(configure =>
            {
                configure.ReturnHttpNotAcceptable = true;
                //// Everyone must be the Admin
                //var policy = new AuthorizationPolicyBuilder()
                //            .RequireAuthenticatedUser()
                //            .RequireRole("Admin")
                //            .Build();
                //configure.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddNewtonsoftJson()
                //.AddXmlDataContractSerializerFormatters()
                .AddApplicationPart(typeof(AssemblyReference).Assembly);

            //builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
            //        builder.Services.AddSwaggerGen(options =>
            //        {
            //            options.SwaggerDoc("v1", new() { Title = "Tournament API", Version = "v1" });

            //            // Add JWT Auth support to Swagger
            //            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            //            {
            //                Name = "Authorization",
            //                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            //                Scheme = "Bearer",
            //                BearerFormat = "JWT",
            //                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            //                Description = "Enter your JWT token like this: Bearer {your token}"
            //            });

            //            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            //{
            //    {
            //        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            //        {
            //            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            //            {
            //                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
            //                Id = "Bearer"
            //            }
            //        },
            //        Array.Empty<string>()
            //    }
            //});
            //        });

            builder.Services.ConfigureOpenApi();


            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            //builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.ConfigureCors();
            builder.Services.ConfigureServiceLayerServices();
            builder.Services.ConfigureRepositories();

            builder.Services.ConfigureJwt(builder.Configuration);

            builder.Services.AddDataProtection();

            builder.Services.AddIdentityCore<ApplicationUser>(opt =>
            {
                opt.Password.RequireLowercase = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 3;
                opt.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<TournamentApiContext>()
                .AddDefaultTokenProviders();

            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AdminPolicy", policy =>
            //        policy.RequireRole("Admin")
            //        .RequireClaim(ClaimTypes.NameIdentifier)
            //        .RequireClaim(ClaimTypes.Role));

            //    options.AddPolicy("EmployeePolicy", policy =>
            //    policy.RequireRole("Employee"));
            //});

            //Add versioning
            builder.Services.AddApiVersioning(setupAction =>
            {
                setupAction.ReportApiVersions = true;
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
            }).AddMvc();


            var app = builder.Build();

            app.ConfigureExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                await app.SeedDataAsync();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
