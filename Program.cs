using ScalarDemo.Extensions;

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
			document.Info.Description = "E-Commerce API description";

			return Task.CompletedTask;
		});
	});
}

builder.Services.AddOpenApi("internal", options =>
{
	options.AddDocumentTransformer((document, context, ct) =>
	{
		var apiDescriptions = context
			.ApplicationServices
			.GetRequiredService<IApiVersionDescriptionProvider>()
			.ApiVersionDescriptions;

		var v1 = apiDescriptions.First(x => x.ApiVersion.MajorVersion == 1);

		document.Info.Title = "Troy API Internal (v1)";
		document.Info.Version = v1.ApiVersion.ToString();
		return Task.CompletedTask;

	});
});

builder.Services.AddOpenApi("publish", options =>
{
	options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		// Lọc endpoints theo version từ context
		var apiVersion = context.ApplicationServices
			.GetRequiredService<IApiVersionDescriptionProvider>()
			.ApiVersionDescriptions
			.FirstOrDefault()?.ApiVersion.ToString() ?? "2.0";

		document.Info.Title = $"Troy API {apiVersion}";
		document.Info.Version = apiVersion;

		return Task.CompletedTask;
	});
});

// Identity
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
	.HasApiVersion(new ApiVersion(1, 0))
	.HasApiVersion(new ApiVersion(2, 0))
	.ReportApiVersions()
	.Build();

if (app.Environment.IsDevelopment())
{
	// Lấy URL local tự động
	var configuration = app.Configuration;
	var localUrl = configuration["urls"] ??
						 configuration["ASPNETCORE_URLS"] ??
						 "http://localhost:5000";

	// OpenAPI endpoints
	app.MapOpenApi("/troy/{documentName}.json");
	app.MapScalarApiReference("/troy", (options) =>
	{
		options.WithTitle("E-Commerce API")
			.WithClassicLayout()
			.WithOpenApiRoutePattern("/troy/{documentName}.json")
			//.HideSearch()
			.ForceDarkMode()
			.ShowOperationId()
			.ExpandAllTags()
			.SortTagsAlphabetically()
			.SortOperationsByMethod()
			.PreserveSchemaPropertyOrder()
			.WithTheme(ScalarTheme.Purple)
			.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
			.WithProxy("https://api-gateway.company.com")
			.AddServer(localUrl, "local")
			.AddServer("https://api.company.com", "Production")
			.AddServer("https://staging-api.company.com", "Staging");
	});
	app.MapGet("/", () => Results.Redirect("/troy/v1")).ExcludeFromDescription();
}

app.MapFallback(() => Results.Redirect("/troy/v1"));
app.UseHttpsRedirection();

// Map API endpoints

app.MapEndpoints();

app.MapCatalogApiV1(versionSet);
app.MapCatalogApiV2(versionSet);

app.Run();
