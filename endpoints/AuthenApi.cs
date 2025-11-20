using Microsoft.AspNetCore.Identity;

namespace ScalarDemo.endpoints;

internal static class AuthenApi
{
    public static IEndpointRouteBuilder MapAuthenApi(this IEndpointRouteBuilder app)
    {
        var vApi = app.NewVersionedApi("Authen");
        var api = vApi.MapGroup("api/Authen")
                                    .HasApiVersion(1, 0)
                                    .HasApiVersion(2, 0);
        var v1 = vApi.MapGroup("api/Authen").HasApiVersion(1, 0);
        var v2 = vApi.MapGroup("api/Authen").HasApiVersion(2, 0);

        v2.MapIdentityApi<IdentityUser>();
        return app;
    }
}