using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;
using ScalarDemo.Extensions;
using ScalarDemo.helper.enums;
using ScalarDemo.Helper;
using ScalarDemo.Service;
using ScalarDemo.Service.Implement;
//using static ScalarDemo.Common.Constants;

var builder = WebApplication.CreateBuilder(args);

// DB Context

#region DB Context

builder.Services.AddDbContext<ApplicationDbContext>(
	options => options.UseSqlServer("Server=BE015;Database=TestScalar;User Id=sa;Password=1234567;MultipleActiveResultSets=True;TrustServerCertificate=true;"));

#endregion DB Context

// API Versioning

#region API Versioning

builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1, 0);
	options.ReportApiVersions = true;
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ApiVersionReader = ApiVersionReader.Combine(
		 new UrlSegmentApiVersionReader()
		);
})
				.AddApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});

#endregion API Versioning

// OpenAPI + Versions

#region OpenAPI + Versions

builder.Services.AddEndpointsApiExplorer();
string[]? a = new[] { "v1", "v2", "v3", "v2.2" };

foreach (var version in Contants.ApiVersions.All)
{
	var vs = version.AsOpenApiName();
	builder.Services.AddOpenApi(vs, _ =>
	{
		_.AddDocumentTransformer((document, context, cancellationToken) =>
		{
			document.Servers.Clear();
			document.Info.Title = $"Troy {vs.ToUpper()}";
			document.Info.Version = $"{vs.ToUpper()}";
			document.Info.Description = "E-Commerce Troy API description";

			return Task.CompletedTask;
		});
		_.AddScalarTransformers();
	});
}

#endregion OpenAPI + Versions

// Identity

#region Identity

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

#endregion Identity

// Scalar Options

#region Scalar Options

builder.Services
		.AddOptions<ScalarOptions>()
		.BindConfiguration("Scalar")
		.PostConfigure(options => { })
		.ValidateDataAnnotations()
		.ValidateOnStart();

#endregion Scalar Options

// RateLimiter

#region RateLimiter

builder.Services.AddRateLimiter(_ =>
{
	_.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

	_.OnRejected = async (context, cancellationToken) =>
	{
		// Custom rejection handling logic
		context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
		context.HttpContext.Response.Headers["Retry-After"] = "60";

		await context.HttpContext.Response.WriteAsJsonAsync("Rate limit exceeded. Please try again later.", cancellationToken);
	};

	_.AddPolicy("otp", httpContext =>
	{
		var userId = httpContext.User.Identity?.Name ?? "anonymous";

		return RateLimitPartition.GetSlidingWindowLimiter(
			partitionKey: userId,
			factory: _ => new SlidingWindowRateLimiterOptions
			{
				PermitLimit = 3,
				Window = TimeSpan.FromMinutes(1),
				SegmentsPerWindow = 3,
				QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
				QueueLimit = 0
			});
	});
});

#endregion RateLimiter

// DI

#region DI

builder.Services.AddKeyedSingleton<ITemplateService, FluidTemplateRenderer>(TemplateEngine.Fluid);

builder.Services.AddKeyedSingleton<ITemplateService, RazorLighTemplateRender>(TemplateEngine.Razor);

#endregion DI

// Fluid

#region Fluid

//builder.Services.AddSingleton<TemplateOptions>(sp =>
//{
//	var options = new TemplateOptions();
//	options.MemberAccessStrategy
//	   .Register<WelcomeEmailModel>(m => m.Date);

//	return options;
//});

#endregion Fluid

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	// Lấy URL local tự động
	var configuration = app.Configuration;
	var localUrl = configuration["urls"] ??
						 configuration["ASPNETCORE_URLS"] ??
						 "http://localhost:5000";

	var scalarOptions = app.Services.GetRequiredService<IOptions<ScalarOptions>>().Value;
	var routePattern = scalarOptions.OpenApiRoutePattern;

	// OpenAPI endpoints
	app.MapOpenApi(routePattern);
	app.MapScalarApiReference((options) =>
	{
		options.AddServer(localUrl, "local");
	});

	app.MapGet("/", () => Results.Redirect($"/troy/{Contants.ApiVersions.V1_0.AsOpenApiName()}")).ExcludeFromDescription();
}

app.MapFallback(() => Results.Redirect($"/troy/{Contants.ApiVersions.V1_0.AsOpenApiName()}"));
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter();

// Map API endpoints
app.MapEndpointGroups();
app.Run();
