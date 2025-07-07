using Services.Contracts;
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

            //services.AddLazy<ITournamentService>();

            services.AddScoped(provider => new Lazy<ITournamentService>(() => provider.GetRequiredService<ITournamentService>()));
            services.AddScoped(provider => new Lazy<IGameService>(() => provider.GetRequiredService<IGameService>()));

        }


        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITournamentRepository, TournamentRepository>();
            services.AddScoped<IGameRepository, GameRepository>();

            services.AddScoped(provider => new Lazy<ITournamentRepository>(() => provider.GetRequiredService<ITournamentRepository>()));
            services.AddScoped(provider => new Lazy<IGameRepository>(() => provider.GetRequiredService<IGameRepository>()));


        }

        //public static class ServiceCollectionExtensions
        //{
        //    public static IServiceCollection AddLazy<TService>(this IServiceCollection services) where TService : class
        //    {
        //        return services.AddScoped(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));
        //    }

        //}

    }
}
