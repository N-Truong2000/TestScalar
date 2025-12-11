namespace ScalarDemo.Extensions;

public sealed class VersionedEndpointContext
{
	public RouteGroupBuilder All { get; }
	public IReadOnlyDictionary<ApiVersion, RouteGroupBuilder> Versions { get; }

	public VersionedEndpointContext(RouteGroupBuilder all, IDictionary<ApiVersion, RouteGroupBuilder> versions)
	{
		All = all;
		Versions = new Dictionary<ApiVersion, RouteGroupBuilder>(versions);
	}

	public RouteGroupBuilder For(ApiVersion version)
		=> Versions[version];

	public bool TryFor(ApiVersion version, out RouteGroupBuilder group)
		=> Versions.TryGetValue(version, out group);
}

public static class VersionedApiExtensions
{
	public static IEndpointRouteBuilder MapVersionedApi(this IEndpointRouteBuilder app,
														 string name,
														 string routePrefix,
														 ApiVersion[] versions,
														 params Action<VersionedEndpointContext>[] endpoints)
	{
		var vApi = app.NewVersionedApi(name);

		var all = vApi.MapGroup(routePrefix);

		var versionGroups = new Dictionary<ApiVersion, RouteGroupBuilder>();

		foreach (var version in versions)
		{
			all.HasApiVersion(version);

			var vg = vApi.MapGroup(routePrefix)
										.HasApiVersion(version);

			versionGroups[version] = vg;
		}

		var ctx = new VersionedEndpointContext(all, versionGroups);

		foreach (var endpoint in endpoints)
		{
			endpoint(ctx);
		}

		return app;
	}
}
