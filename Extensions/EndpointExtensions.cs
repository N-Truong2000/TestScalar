using System.Reflection;
using Asp.Versioning.Builder;
using ScalarDemo.endpoints.Shared;

namespace ScalarDemo.Extensions;

public static class EndpointExtensions
{
	public static RouteGroupBuilder VersionedGroup(this IVersionedEndpointRouteBuilder vApi,
												string featureName,
												string groupName,
												ApiVersion apiVersion,
												bool requireAuth = false)
	{
		var group = vApi.MapGroup($"api/v{{version:apiVersion}}/{featureName.ToLower()}")
						.WithGroupName(groupName)
						.HasApiVersion(apiVersion)
						.WithTags(featureName);

		if (requireAuth)
			group.RequireAuthorization();

		return group;
		;
	}

	public static RouteGroupBuilder SharedGroup(this IVersionedEndpointRouteBuilder vApi,
											 string featureName,
											 params ApiVersion[] versions)
	{
		var group = vApi.MapGroup($"api/v{{version:apiVersion}}/{featureName.ToLower()}")
										.WithTags(featureName);

		foreach (var version in versions)
			group.HasApiVersion(version);

		return group;
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
	public static void MapEndpointGroups(this WebApplication app)
	{
		var groups = typeof(EndpointGroupBase).Assembly
			.GetTypes()
			.Where(t => !t.IsAbstract && typeof(EndpointGroupBase).IsAssignableFrom(t))
			.Select(Activator.CreateInstance)
			.Cast<EndpointGroupBase>();

		foreach (var group in groups)
			group.Map(app);
	}
}
