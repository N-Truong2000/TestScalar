using System.Reflection;
using Asp.Versioning.Builder;
using ScalarDemo.endpoints.Shared;

namespace ScalarDemo.Extensions;

public static class EndpointExtensions
{
	public static RouteGroupBuilder VersionedGroup(this IVersionedEndpointRouteBuilder vApi, string featureName, string groupName, ApiVersion apiVersion, bool requireAuth = false)
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
