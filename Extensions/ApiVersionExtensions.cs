namespace ScalarDemo.Extensions;

public static class ApiVersionExtensions
{
	public static string AsOpenApiName(this ApiVersion v)
	{
		return v.MinorVersion == 0
		? $"v{v.MajorVersion}"
		: $"v{v.MajorVersion}.{v.MinorVersion}";
	}
}
