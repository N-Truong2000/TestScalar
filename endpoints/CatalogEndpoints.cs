using System.Reflection;
using ScalarDemo.Attributes;
using ScalarDemo.Common;
using ScalarDemo.endpoints.Shared;
using ScalarDemo.Extensions;
using ScalarDemo.helper.enums;
using ScalarDemo.Service;

[assembly: AssemblyOwner("Nguyen Van A")]

namespace ScalarDemo.endpoints;

public class CatalogEndpoints : EndpointGroupBase
{
	private string _name = "Catalog";

	public override void Map(WebApplication app)
	{
		var vApi = app.NewVersionedApi(_name);

		var api = vApi.SharedGroup(_name, new ApiVersion(1, 0), new ApiVersion(2, 0));

		var v1 = vApi.VersionedGroup(_name, Constants.ApiVersion.V1, new ApiVersion(1, 0)).Stable();

		var v2 = vApi.VersionedGroup(_name, Constants.ApiVersion.V2, new ApiVersion(2, 0)).Experimental();

		MapItemsEndpoints(v1, v2);
		MapEmailEndpoints(v1);
		MapHealthEndpoints(api);
		MapAssemblyEndpoints(v2);
		;
	}

	private void MapItemsEndpoints(RouteGroupBuilder v1, RouteGroupBuilder v2)
	{
		v1.MapGet("/items", () => new[] { "Item 1", "Item 2", "Item 3" })
			.WithName("ListItemsV1")
			.WithSummary("List catalog items (v1)")
			.WithDescription("Get a list of catalog items using API v1.")
			.WithTags("Items")
			.WithBadge("Alpha")
			.WithBadge("Beta", BadgePosition.Before)
			.WithBadge("Internal", BadgePosition.After, "#ff6b35");

		v2.MapGet("/items", () => new[] { "Item A", "Item B" })
			.WithName("ListItemsV2")
			.WithSummary("List catalog items (v2)")
			.WithDescription("Get a list of catalog items using API v2.")
			.WithTags("Items");
	}

	private void MapEmailEndpoints(RouteGroupBuilder v1)
	{
		v1.MapGet("/email-preview", async ([FromKeyedServices(TemplateEngine.Razor)] ITemplateService templateService) =>
		{
			var model = new
			{
				Name = "Sơn",
				Date = DateTime.Now,
				DashboardUrl = "https://chatgpt.com/c/68f48ca5-0a3c-8320-b7de-3ca7e0a45e50"
			};

			string html = await templateService.RenderAsync("WelcomeEmail.cshtml", model);
			return TypedResults.Content(html, "text/html");
		})
		.WithName("EmailPreviewV1")
		.WithSummary("Preview welcome email template (v1)")
		.WithDescription("Render the welcome email template using RazorLight and return HTML output.")
		.WithTags("Email")
		.WithBadge("Template", BadgePosition.After)
		/*.RequireRateLimiting("otp")*/;

		v1.MapGet("/email-preview-flid", async ([FromKeyedServices(TemplateEngine.Fluid)] ITemplateService templateService) =>
		{
			var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TemplateFluid.html");
			var template = System.IO.File.ReadAllText(path);
			var model = new
			{
				Name = "John Doe",
				Date = DateTime.Now,
				DashboardUrl = "https://app.example.com/dashboard"
			};

			var html = await templateService.RenderAsync(template, model);
			return TypedResults.Content(html, "text/html");
		})
		.WithName("Email Preview flid V1")
		.WithSummary("Preview welcome email template (v1)")
		.WithDescription("Render the welcome email template using RazorLight and return HTML output.")
		.WithTags("Email")
		.WithBadge("Template", BadgePosition.After);
		//.RequireRateLimiting("otp");
	}

	private void MapHealthEndpoints(RouteGroupBuilder api)
	{
		api.MapGet("/health", () =>
		{
			var a = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);
			var b = new DateTime(2025, 1, 1, 17, 0, 0, DateTimeKind.Local); // GMT+7
			return Results.Ok(a == b);
		})
		.WithName("CatalogHealth")
		.WithTags("Health");
	}

	private void MapAssemblyEndpoints(RouteGroupBuilder v2)
	{
		v2.MapGet("/Assembly", () =>
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "ScalarDemo.Images.logo.png";

			if (!assembly.GetManifestResourceNames().Contains(resourceName))
				return TypedResults.NotFound("Resource not found");

			var stream = assembly.GetManifestResourceStream(resourceName);
			if (stream == null)
				return Results.NotFound("Resource not found");

			return TypedResults.File(stream, "image/png", "logo.png");
		})
		.WithName("Assembly")
		.WithTags("Health");
	}
}
