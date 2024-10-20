using Application.Requests.Titles.Queries.GetTitles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Endpoints.Titles
{
    public class GetTitles : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("titles/title", async (
                string titleName,
                string titleId,
                [FromServices] ISender sender) =>
            {
                var request = new GetTitlesQuery(titleName, titleId);

                var result = await sender.Send(request);

                if (result is null)
                    return Results.StatusCode(500);

                return Results.Json(result);
            });
        }
    }
}
