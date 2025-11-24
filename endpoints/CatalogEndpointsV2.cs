using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;

namespace ScalarDemo.endpoints;

public static class CatalogEndpointsV2
{
    public static IEndpointRouteBuilder MapCatalogApiV2(
        this IEndpointRouteBuilder app,
        ApiVersionSet versionSet)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/products")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(2, 0)
            .WithGroupName("publish")
            .WithTags("Products V2")
            .WithOpenApi();

        group.MapGet("/", GetProducts)
            .WithName("GetProductsV2")
            .WithSummary("Get all products (V2)")
            .WithDescription("Retrieves a list of all products with extended information");

        group.MapGet("/{id:int}", GetProductById)
            .WithName("GetProductByIdV2");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProductV2");

        // V2 new feature: Bulk operations
        group.MapPost("/bulk", BulkCreateProducts)
            .WithName("BulkCreateProductsV2")
            .WithSummary("Bulk create products")
            .WithDescription("Creates multiple products at once (V2 only)");

        group.MapGet("/search", SearchProducts)
            .WithName("SearchProductsV2")
            .WithSummary("Search products")
            .WithDescription("Search products by name or category (V2 only)");

        return app;
    }

    private static IResult GetProducts()
    {
        var products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 999.99, Stock = 10, Category = "Electronics", Rating = 4.5 },
            new { Id = 2, Name = "Mouse", Price = 29.99, Stock = 50, Category = "Accessories", Rating = 4.2 }
        };
        return Results.Ok(products);
    }

    private static IResult GetProductById(int id)
    {
        var product = new { Id = id, Name = "Laptop", Price = 999.99, Stock = 10, Category = "Electronics", Rating = 4.5 };
        return Results.Ok(product);
    }

    private static IResult CreateProduct([FromBody] CreateProductV2Request request)
    {
        var product = new { Id = 3, request.Name, request.Price, request.Stock, request.Category };
        return Results.Created($"/api/v2/products/{product.Id}", product);
    }

    private static IResult BulkCreateProducts([FromBody] CreateProductV2Request[] requests)
    {
        var products = requests.Select((r, i) => new { Id = i + 1, r.Name, r.Price, r.Stock, r.Category });
        return Results.Ok(new { Created = products.Count(), Products = products });
    }

    private static IResult SearchProducts([FromQuery] string? query, [FromQuery] string? category)
    {
        var products = new[]
        {
            new { Id = 1, Name = "Laptop", Price = 999.99, Category = "Electronics" },
            new { Id = 2, Name = "Mouse", Price = 29.99, Category = "Accessories" }
        };
        return Results.Ok(products);
    }
}

public record CreateProductV2Request(string Name, decimal Price, int Stock, string Category);