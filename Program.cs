using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ScalarDemo.data;
using ScalarDemo.endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer("Server=BE015;Database=TestScalar;User Id=sa;Password=1234567;MultipleActiveResultSets=True;TrustServerCertificate=true;"));
builder.Services.AddEndpointsApiExplorer();
string[] versions = ["v1", "v2"];
foreach (var version in versions)
{
    builder.Services.AddOpenApi(version, options =>
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Servers = [];
            document.Info.Title = $"Troy {version.ToUpper()}";
            document.Info.Version = $"AAAAAAAAAAAAAAAAA {version.ToUpper()}";
            document.Info.Description = "eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee";
            return Task.CompletedTask;
        });
    });
}
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("troy", (options, httpContext) =>
    {
        var isAdmin = httpContext.User.IsInRole("Admin");
        options.WithTitle(isAdmin ? "Admin API" : "Public API");
        options.WithTitle("My API");
    });
    app.MapGet("/", () => Results.Redirect("/troy/internal")).ExcludeFromDescription();
}
app.MapFallback(() => Results.Redirect("/scalar/v1"));
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast").WithOpenApi();
app.MapGet("/hello", () => "Hello").WithOpenApi();
app.MapGet("/hi", () => "Hi").WithOpenApi();
// New endpoint bạn vừa tạo https://localhost:7201/openapi/v1.json
app.MapGet("/users/{id}", (int id) =>
{
    return new UserDto(id, "Alice");
})
.WithName("GetUserById")
.WithOpenApi();
app.MapCatalogApi();
app.MapAuthenApi();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
record UserDto(int Id, string Name);