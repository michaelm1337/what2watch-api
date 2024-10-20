using Application.Requests.Content.Queries.GetContents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.Content
{
    public class GetContents : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("content/all", async (
                int page,
                int pageSize,
                [FromServices] IMediator mediator,
                CancellationToken cancellationToken) =>
                {
                    var query = new GetContentsQuery(
                        page,
                        pageSize);

                    var result = await mediator.Send(query, cancellationToken);

                    if (result is null)
                        return Results.StatusCode(500);

                    return Results.Json(result, statusCode: (int)result.StatusCode);
                })
                .WithTags(Tags.Content)
                .WithDescription("Endpoint responsible for retrieving all contents by pagination.");
        }
    }
}
