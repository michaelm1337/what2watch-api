using Domain.Shared;
using Domain.ValueObjects;
using MediatR;

namespace Application.Requests.Content.Commands.CreateContent
{
    public sealed record CreateContentCommand(string Id, string Title, string Description, Poster Poster) : IRequest<Result>;
}
