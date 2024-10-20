using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface IImdbRepository
    {
        Task<List<Title>> GetTitles(Func<Title, bool> func);
        Task InsertTitles(List<Title> titles);
        Task InsertCrews(List<Crew> crews);
        Task InsertEpisodes(List<Episode> episodes);
        Task InsertRatings(List<Rating> ratings);
        Task InsertPrincipals(List<Principal> principals);
        Task InsertPersons(List<Person> persons);
    }
}
