using ScalarDemo.endpoints.Shared;
using ScalarDemo.Extensions;
using ScalarDemo.Helper;

namespace ScalarDemo.Endpoints;

public class ProductEndpoins : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		app.MapVersionedApi(
		name: "Product",
		routePrefix: "api/v{version:apiVersion}/Product",
	   versions: Contants.ApiVersions.All,
		GetItemById
		);
	}



	private Action<VersionedEndpointContext> GetItemById =
			ctx =>
			{

				ctx.All.MapGet("/items/{idd}", () => "Item 1")
						.WithName("GetItem")
						.WithTags("Product");
				ctx.All.MapGet("/items/by/{name}", () => "Item 2621")
						.WithName("GetItem123")
						.WithTags("Product");
			};

}
