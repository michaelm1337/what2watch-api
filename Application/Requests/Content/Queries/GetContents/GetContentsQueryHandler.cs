using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;
using MediatR;
using System.Net;

namespace Application.Requests.Content.Queries.GetContents
{
    internal sealed class GetContentsQueryHandler : IRequestHandler<GetContentsQuery, PagedResult<ContentEntity>>
    {
        private readonly IContentRepository _contentRepository;

        public GetContentsQueryHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<PagedResult<ContentEntity>> Handle(GetContentsQuery query, CancellationToken cancellationToken)
        {
            var pagedResult = await _contentRepository.GetAllContentsAsync(query.Page, query.PageSize, cancellationToken);

            if (pagedResult.Items is null || pagedResult.Items.Count == 0)
            {
                pagedResult.StatusCode = HttpStatusCode.NoContent;

                return pagedResult;
            }

            pagedResult.StatusCode = HttpStatusCode.OK;

            return pagedResult;
        }
    }
}
