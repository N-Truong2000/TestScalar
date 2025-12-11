using ScalarDemo.endpoints.Shared;

namespace ScalarDemo.endpoints;

public class AuthEndpoints : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		var vApi = app.NewVersionedApi("Authen1");

		var v2 = vApi.MapGroup("api/v{version:apiVersion}/authen")
				.WithGroupName("v2")
				.HasApiVersion(2, 0);

		// Identity API ở V2
		v2.MapIdentityApi<IdentityUser>().Stable();
	}
}
