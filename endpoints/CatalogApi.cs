namespace ScalarDemo.endpoints;

public static class CatalogApi
{
	public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder app)
	{
		var vApi = app.NewVersionedApi("Catalog");
		var api = vApi.MapGroup("api/v{version:apiVersion}/catalog")
									.HasApiVersion(1, 0)
									.HasApiVersion(2, 0);
		var v1 = vApi.MapGroup("api/v{version:apiVersion}/catalog")
									.WithGroupName("v1")
									.HasApiVersion(1, 0);
		var v2 = vApi.MapGroup("api/v{version:apiVersion}/catalog")
									.WithGroupName("v2")
									.HasApiVersion(2, 0);

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
		  .WithName("health")
		  .WithTags("Health");

		return app;
	}
}