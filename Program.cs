using Microsoft.Extensions.Options;
using ScalarDemo.Extensions;
using ScalarDemo.Service;

var builder = WebApplication.CreateBuilder(args);

// DB Context
builder.Services.AddDbContext<ApplicationDbContext>(
	options => options.UseSqlServer("Server=BE015;Database=TestScalar;User Id=sa;Password=1234567;MultipleActiveResultSets=True;TrustServerCertificate=true;"));

// API Versioning
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1);
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

// OpenAPI + Versions
builder.Services.AddEndpointsApiExplorer();

var versions = builder.Configuration.GetSection("ApiVersions").Get<string[]>() ?? Array.Empty<string>();
foreach (var version in versions)
{
	builder.Services.AddOpenApi(version, options =>
	{
		options.AddDocumentTransformer((document, context, cancellationToken) =>
		{
			document.Servers.Clear();
			document.Info.Title = $"Troy {version.ToUpper()}";
			document.Info.Version = $"{version.ToUpper()}";
			document.Info.Description = "E-Commerce Troy API description";

			return Task.CompletedTask;
		});
		options.AddScalarTransformers();
	});
}

// Identity
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
		.AddOptions<ScalarOptions>()
		.BindConfiguration("Scalar")
		.PostConfigure(options => { })
		.ValidateDataAnnotations()
		.ValidateOnStart();
// DI
builder.Services.AddSingleton<IEmailTemplateService, EmailTemplateService>();

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

	app.MapGet("/", () => Results.Redirect("/troy/v1")).ExcludeFromDescription();
}

app.MapFallback(() => Results.Redirect("/troy/v1"));
app.UseHttpsRedirection();
app.UseStaticFiles();

// Map API endpoints
app.MapEndpoints();

app.Run();
