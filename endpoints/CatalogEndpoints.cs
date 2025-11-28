using ScalarDemo.Common;
using ScalarDemo.endpoints.Shared;
using ScalarDemo.Extensions;
using ScalarDemo.Service;

namespace ScalarDemo.endpoints;

public class CatalogEndpoints : EndpointGroupBase
{
	private string _name = "Catalog";

	public override void Map(WebApplication app)
	{
		var vApi = app.NewVersionedApi(_name);

		var api = vApi.MapGroup("api/v{version:apiVersion}/catalog")
			.HasApiVersion(new ApiVersion(1, 0))
			.HasApiVersion(new ApiVersion(2, 0));

		var v1 = vApi.VersionedGroup(_name, Constants.ApiVersion.V1, new ApiVersion(1, 0)).Stable();

		var v2 = vApi.VersionedGroup(_name, Constants.ApiVersion.V2, new ApiVersion(2, 0)).Experimental();

		v1.MapGet("/items", () => new[] { "Item 1", "Item 2", "Item 3" })
				.WithName("ListItemsV1")
				.WithSummary("List catalog items (v1)")
				.WithDescription("Get a list of catalog items using API v1.")
				.WithTags("Items").WithBadge("Alpha")
				.WithBadge("Beta", BadgePosition.Before)
				.WithBadge("Internal", BadgePosition.After, "#ff6b35");

		v2.MapGet("/items", () => new[] { "Item A", "Item B" })
					.WithName("ListItemsV2")
					.WithSummary("List catalog items (v2)")
					.WithDescription("Get a list of catalog items using API v2.")
					.WithTags("Items");
		v1.MapGet("/email-preview", async (IEmailTemplateService templateService) =>
		{
			var model = new
			{
				Name = "Sơn",
				Date = DateTime.Now,
				DashboardUrl = "https://example.com/dashboard"
			};

			string html = await templateService.RenderAsync("WelcomeEmail.cshtml", model);

			return Results.Content(html, "text/html");
		})
			.WithName("EmailPreviewV1")
			.WithSummary("Preview welcome email template (v1)")
			.WithDescription("Render the welcome email template using RazorLight and return HTML output.")
			.WithTags("Email")
			.WithBadge("Template", BadgePosition.After);

		api.MapGet("/health", () => "OK")
		  .WithName("CatalogHealth")
		  .WithTags("Health");
	}
}
