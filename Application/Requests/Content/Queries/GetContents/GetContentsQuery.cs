using Domain.Entities;
using Domain.Shared;
using MediatR;

namespace Application.Requests.Content.Queries.GetContents
{
    public sealed record GetContentsQuery(int Page, int PageSize) : IRequest<PagedResult<ContentEntity>>;
}
