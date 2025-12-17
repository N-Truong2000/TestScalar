using ScalarDemo.endpoints.Shared;
using ScalarDemo.Extensions;
using ScalarDemo.Helper;

namespace ScalarDemo.Endpoints;

// Renamed to 'ProductEndpointsV1' to avoid duplicate definition.
public partial class ProductEndpointsV2 : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		app.MapVersionedApi(
		name: "Product",
		routePrefix: "api/v{version:apiVersion}/Product",
		versions: [Contants.ApiVersions.V2_0]
		, GetItemByIdV2
		);
	}

	private Action<VersionedEndpointContext> GetItemByIdV2 =
			ctx =>
			{
				ctx.For(Contants.ApiVersions.V2_0)
				   .MapGet("/itemsnghe", () => "22222222222222222222222222222 2222222")
				   .WithTags("Product")
				   .WithName("ListItems-V2");
			};
}
