using Domain.Entities;
using Domain.Interfaces;
using Domain.Shared;
using MediatR;
using System.Net;

namespace Application.Requests.Content.Commands.CreateContent
{
    internal sealed class CreateContentCommandHandler : IRequestHandler<CreateContentCommand, Result>
    {
        private readonly IContentRepository _contentRepository;

        public CreateContentCommandHandler(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public async Task<Result> Handle(CreateContentCommand command, CancellationToken cancellationToken)
        {
            Result result = new Result();

            bool inserted = await _contentRepository.CreateContentAsync(new ContentEntity
            {
                Description = command.Description,
                Poster = command.Poster,
                Title = command.Title
            }, cancellationToken);

            if (!inserted)
            {
                result.StatusCode = HttpStatusCode.InternalServerError;
                result.Error = new Error()
                {
                    ErrorMessage = "Não foi possível adicionar o conteúdo na lista."
                };

                return result;
            }

            result.StatusCode = HttpStatusCode.OK;

            return result;
        }
    }
}
