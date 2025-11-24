using System.Reflection;
using Asp.Versioning.Builder;
using ScalarDemo.Extensions;

namespace ScalarDemo.Extensions;

public static class EndpointExtensions
{
	public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
	{
		var groupName = group.GetType().Name;

		return app
			.MapGroup($"/api/{groupName}")
		.WithGroupName(groupName)
		.WithTags(groupName);
	}

	public static RouteGroupBuilder CreateVersionedGroup(this IVersionedEndpointRouteBuilder vApi, string nameUrl, string groupName, int majorVersion, int minorVersion = 0)
	{
		return vApi.MapGroup($"api/v{version:apiVersion}/{nameUrl.ToLower()}")
				   .WithGroupName(groupName)
				   .HasApiVersion(majorVersion, minorVersion);
	}

	public static WebApplication MapEndpoints(this WebApplication app)
	{
		var endpointGroupType = typeof(EndpointGroupBase);

		var assembly = Assembly.GetExecutingAssembly();

		var endpointGroupTypes = assembly.GetExportedTypes()
			.Where(t => t.IsSubclassOf(endpointGroupType));

		foreach (var type in endpointGroupTypes)
		{
			if (Activator.CreateInstance(type) is EndpointGroupBase instance)
			{
				instance.Map(app);
			}
		}

		return app;
	}
}
