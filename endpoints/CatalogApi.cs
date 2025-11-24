using ScalarDemo.Common;
using ScalarDemo.Extensions;

namespace ScalarDemo.endpoints;

public class CatalogApi : EndpointGroupBase
{
	private string _name = "Catalog";
	public override void Map(WebApplication app)
	{
		var vApi = app.NewVersionedApi(_name);


		var api = vApi.MapGroup("api/v{version:apiVersion}/catalog")
			.HasApiVersion(new ApiVersion(1, 0))
			.HasApiVersion(new ApiVersion(2, 0));

		var v1 = vApi.CreateVersionedGroup(_name, Constants.ApiVersion.V1, new ApiVersion(1, 0));

		var v2 = vApi.CreateVersionedGroup(_name, Constants.ApiVersion.V2, new ApiVersion(2, 0));

		v1.MapGet("/items", () => new[] { "Item 1", "Item 2", "Item 3" })
				.WithName("ListItemsV1")
				.WithSummary("List catalog items (v1)")
				.WithDescription("Get a list of catalog items using API v1.")
				.WithTags("Items");

		v2.MapGet("/items", () => new[] { "Item A", "Item B" })
					.WithName("ListItemsV2")
					.WithSummary("List catalog items (v2)")
					.WithDescription("Get a list of catalog items using API v2.")
					.WithTags("Items");

		api.MapGet("/health", () => "OK")
		  .WithName("CatalogHealth")
		  .WithTags("Health");
	}
}
