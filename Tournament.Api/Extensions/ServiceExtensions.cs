﻿using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Contracts;
using System.Reflection;
using System.Text;
using Tournament.Core.Configuration;
using Tournament.Core.Repositories;
using Tournament.Data.Repositories;
using Tournament.Services;

namespace Tournament.Api.Extensions
{
    public static class ServiceExtensions 
    { 
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(builder =>
            {
                builder.AddPolicy("AllowAll", p =>
                p.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
            });
        }

        public static void ConfigureServiceLayerServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<ITournamentService, TournamentService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IAuthService, AuthService>();

            //services.AddLazy<ITournamentService>();

            services.AddScoped(provider => new Lazy<ITournamentService>(() => provider.GetRequiredService<ITournamentService>()));
            services.AddScoped(provider => new Lazy<IGameService>(() => provider.GetRequiredService<IGameService>()));
            services.AddScoped(provider => new Lazy<IAuthService>(() => provider.GetRequiredService<IAuthService>()));

        }


        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITournamentRepository, TournamentRepository>();
            services.AddScoped<IGameRepository, GameRepository>();

            services.AddScoped(provider => new Lazy<ITournamentRepository>(() => provider.GetRequiredService<ITournamentRepository>()));
            services.AddScoped(provider => new Lazy<IGameRepository>(() => provider.GetRequiredService<IGameRepository>()));


        }

        //Add versioning
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            
            //var apiVersionDescriptionProvider = services.BuildServiceProvider()
            //    .GetRequiredService<IApiVersionDescriptionProvider>();

            services.AddApiVersioning(setupAction =>
            {
                setupAction.ReportApiVersions = true;
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
            }).AddMvc()
            .AddApiExplorer(setupAction =>
            {
                setupAction.SubstituteApiVersionInUrl = true;
            }
            );

        }

        // Swagger med tokens
        public static void ConfigureOpenApi(this IServiceCollection services) =>
        services.AddEndpointsApiExplorer()
               .AddSwaggerGen(setup =>
               {
                   //API Versioning
                   var apiVersionDescriptionProvider = services.BuildServiceProvider()
               .GetRequiredService<IApiVersionDescriptionProvider>();

                   foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                   {
                       setup.SwaggerDoc($"{description.GroupName}",
                           new()
                           {
                               Title = "Tournament Info API",
                               Version = description.ApiVersion.ToString(),
                               Description = "The Api for Tournament by Olena Kut"
                           });
                   }

                   // XML for Tournament.Api
                   var apiXml = Path.Combine(AppContext.BaseDirectory, "Tournament.Api.xml");
                   if (File.Exists(apiXml))
                       setup.IncludeXmlComments(apiXml);

                   // XML for Tournament.Presentation
                   var presentationXml = Path.Combine(AppContext.BaseDirectory, "Tournament.Presentation.xml");
                   if (File.Exists(presentationXml))
                       setup.IncludeXmlComments(presentationXml);

                   //XML for Tournament.Core
                   var coreXml = Path.Combine(AppContext.BaseDirectory, "Tournament.Core.xml");
                   if (File.Exists(coreXml))
                       setup.IncludeXmlComments(coreXml);


                   setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                   {
                       In = ParameterLocation.Header,
                       Description = "Place to add JWT with Bearer",
                       Name = "Authorization",
                       Type = SecuritySchemeType.Http,
                       Scheme = "Bearer"
                   });

                   setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                   {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                   });
               });



        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            ArgumentNullException.ThrowIfNull(nameof(jwtSettings));

            var secretKey = configuration["secretkey"];
            ArgumentNullException.ThrowIfNull(secretKey, nameof(secretKey));

            var jwtConfig = new JwtConfiguration();
            jwtSettings.Bind(jwtConfig);

            services.Configure<JwtConfiguration>(options =>
            {
                options.Issuer = jwtConfig.Issuer;
                options.Audience = jwtConfig.Audience;
                options.Expires = jwtConfig.Expires;
                options.SecretKey = secretKey;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateLifetime = true
                };
            });
        }


    }
}
