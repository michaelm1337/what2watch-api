using Application.Requests.Content.Commands.CreateContent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.Content
{
    public class CreateContent : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("content/create", async (
                [FromBody] CreateContentCommand command,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {

                var result = await sender.Send(command, cancellationToken);

                if (result is null)
                    return Results.StatusCode(500);

                return Results.StatusCode((int)result.StatusCode);
            })
                .WithTags(Tags.Content)
                .WithDescription("Endpoint responsible for registering new contents.");
        }
    }
}
