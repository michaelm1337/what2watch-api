using Application;
using Application.Services;
using Application.WebRequests;
using Refit;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(a => a.RegisterServicesFromAssemblies(AssemblyReference.Assembly));

            services.AddRefitClient<IImdbFiles>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://datasets.imdbws.com/");
                });

            //services.AddHostedService<ImdbCrawler>();
            //services.AddHostedService<PosterCrawler>();

            return services;
        }
    }
}
