using MediatR;

namespace Application.Requests.Titles.Queries.GetTitles
{
    public class GetTitlesQueryHandler : IRequestHandler<GetTitlesQuery, string>
    {
        public async Task<string> Handle(GetTitlesQuery request, CancellationToken cancellationToken)
        {
            return "";
        }
    }
}
