using ScalarDemo.endpoints.Shared;
using ScalarDemo.Extensions;
using ScalarDemo.Helper;

namespace ScalarDemo.Endpoints;

// Renamed to 'ProductEndpointsV1' to avoid duplicate definition.
public class ProductEndpointsV1 : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		app.MapVersionedApi(
		name: "Product",
		routePrefix: "api/v{version:apiVersion}/Product",
		versions: [Contants.ApiVersions.V1_0],
		GetAllItems
		);
	}




	private Action<VersionedEndpointContext> GetAllItems =
	ctx =>
	{
		ctx.For(Contants.ApiVersions.V1_0)
		   .MapGet("/items", () => "name hehehe 222")
			.WithTags("Product")
		   .WithName("ListItems");




	};
}
