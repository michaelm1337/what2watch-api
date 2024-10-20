using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("MongoDbSettings__ConnectionString") ?? "";
            var dbName = Environment.GetEnvironmentVariable("MongoDbSettings__DatabaseName") ?? "";

            services.AddSingleton<IMongoClient>(service =>
            {
                return new MongoClient(connectionString);
            });

            services.AddTransient(service =>
            {
                var client = service.GetRequiredService<IMongoClient>();

                return client.GetDatabase(dbName);
            });

            services.AddTransient<IContentRepository, ContentRepository>();
            services.AddTransient<IImdbRepository, ImdbRepository>();
            services.AddTransient<IMoviePosterDbRepository, MoviePosterDbRepository>();

            return services;
        }
    }
}
