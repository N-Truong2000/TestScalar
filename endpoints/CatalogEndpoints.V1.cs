using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;

namespace ScalarDemo.endpoints;

public static class CatalogEndpointsV1
{
	public static IEndpointRouteBuilder MapCatalogApiV1(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
	{
		var group = app.MapGroup("/api/v{version:apiVersion}/products").RequireAuthorization()
			.WithApiVersionSet(versionSet)
			.HasApiVersion(1, 0)
			.WithGroupName("internal")
			.WithTags("Products")
			.WithOpenApi().WithBadge("Alpha")
	.WithBadge("Beta", BadgePosition.Before)
	.WithBadge("Internal", BadgePosition.After, "#ff6b35");

		group.MapGet("/", GetProducts)
			.WithName("GetProductsV1")
			.WithSummary("Get all products")
			.WithDescription("Retrieves a list of all available products");

		group.MapGet("/{id:int}", GetProductById)
			.WithName("GetProductByIdV1")
			.WithSummary("Get product by ID")
			.WithDescription("Retrieves a specific product by its ID");

		group.MapPost("/", CreateProduct)
			.WithName("CreateProductV1")
			.WithSummary("Create new product")
			.WithDescription("Creates a new product in the catalog");

		group.MapPut("/{id:int}", UpdateProduct)
			.WithName("UpdateProductV1")
			.WithSummary("Update product")
			.WithDescription("Updates an existing product");

		group.MapDelete("/{id:int}", DeleteProduct)
			.WithName("DeleteProductV1")
			.WithSummary("Delete product")
			.WithDescription("Deletes a product from the catalog");

		return app;

		// #DCDCAA
	}

	private static IResult GetProducts()
	{
		var products = new[]
		{
			new { Id = 1, Name = "Laptop", Price = 999.99, Stock = 10 },
			new { Id = 2, Name = "Mouse", Price = 29.99, Stock = 50 }
		};
		return Results.Ok(products);
	}

	private static IResult GetProductById(int id)
	{
		var product = new { Id = id, Name = "Laptop", Price = 999.99, Stock = 10 };
		return TypedResults.Ok(product);
	}

	private static IResult CreateProduct([FromBody] CreateProductRequest request)
	{
		var product = new { Id = 3, request.Name, request.Price, request.Stock };
		return TypedResults.Created($"/api/v1/products/{product.Id}", product);
	}

	private static IResult UpdateProduct(int id, [FromBody] UpdateProductRequest request)
	{
		var product = new { Id = id, request.Name, request.Price, request.Stock };
		return TypedResults.Ok(product);
	}

	private static IResult DeleteProduct(int id)
	{
		return TypedResults.NoContent();
	}
}

public record CreateProductRequest(string Name, decimal Price, int Stock);
public record UpdateProductRequest(string Name, decimal Price, int Stock);
