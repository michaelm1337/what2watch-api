using MediatR;

namespace Application.Requests.Titles.Queries.GetTitles
{
    public sealed record GetTitlesQuery(string TitleName, string TitleId) : IRequest<string>;
}
