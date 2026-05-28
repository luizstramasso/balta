using Fina.Api.Common.Api;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;

namespace Fina.Api.Endpoints.Categories;

public class GetCategoryByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", HandleAsync)
            .WithName("Categories: GetById")
            .WithSummary("Obtém uma categoria existente.")
            .WithDescription("Obtém uma categoria existente com base no ID fornecido.")
            .WithOrder(4)
            .Produces<Response<Category?>>();
    }
    private static async Task<IResult> HandleAsync(
        ICategoryHandler handler,
        long id)
    {
        var request = new GetCategoryByIdRequest
        {
            Id = id,
            UserId = ApiConfiguration.UserId
        };

        var response = await handler.GetByIdAsync(request);
        return response.IsSuccess
            ? TypedResults.Ok(response)
            : TypedResults.BadRequest(response);
    }
}