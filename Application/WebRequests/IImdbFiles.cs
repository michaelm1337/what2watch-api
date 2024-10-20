using Refit;

namespace Application.WebRequests
{
    public interface IImdbFiles
    {
        [Get("/title.basics.tsv.gz")]
        Task<Stream> GetTitleBasics();

        [Get("/title.crew.tsv.gz")]
        Task<Stream> GetTitleCrew();

        [Get("/title.episode.tsv.gz")]
        Task<Stream> GetTitleEpisodes();

        [Get("/title.ratings.tsv.gz")]
        Task<Stream> GetTitleRatings();

        [Get("/title.principals.tsv.gz")]
        Task<Stream> GetPrincipals();

        [Get("/name.basics.tsv.gz")]
        Task<Stream> GetPersons();
    }
}
