using ScalarDemo.Extensions;

namespace ScalarDemo.Endpoints;

public static class ProductEndpoins
{
	public static IEndpointRouteBuilder MapProductApi(this IEndpointRouteBuilder app)
	{
		return app.MapVersionedApi(
		name: "Product",
		routePrefix: "api/Product",
		versions:
		[
			new ApiVersion(1, 0),
		new ApiVersion(2, 0),
		new ApiVersion(3, 0),
		],
		GetAllItems,
		GetItemById,
		GetItemsByNameV1
	);
	}

	private static VersionedEndpoint GetItemById =
	ctx =>
	{
		ctx.All.MapGet("/items/{id:int}", () => "Item 1")
			.WithName("GetItem")
			.WithTags("Items");
	};

	private static VersionedEndpoint GetItemsByNameV1 =
	ctx =>
	{
		ctx.For(new ApiVersion(1, 0))
		   .MapGet("/items/by/{name}", () => "name hehehe 123")
		   .WithTags("Items");
	};

	private static VersionedEndpoint GetAllItems =
	ctx =>
	{
		ctx.For(new ApiVersion(1, 0))
		   .MapGet("/items", () => "name hehehe 222")
		   .WithName("ListItems");

		ctx.For(new ApiVersion(2, 0))
		   .MapGet("/items", () => "name hehehe 333")
		   .WithName("ListItems-V2");

		// Sau này thêm v3 KHÔNG cần đổi delegate
		// ctx.For(new ApiVersion(3, 0)).MapGet(...)
	};
}
