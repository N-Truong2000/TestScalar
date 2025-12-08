using RazorLight;

namespace ScalarDemo.Service;

public interface IEmailTemplateService
{
	Task<string> RenderAsync<T>(string templateName, T model);
}

public class EmailTemplateService : IEmailTemplateService
{
	private readonly RazorLightEngine _engine;
	public EmailTemplateService()
	{
		var root = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

		if (!Directory.Exists(root))
		{
			throw new DirectoryNotFoundException($"EmailTemplates folder not found at: {root}");
		}

		_engine = new RazorLightEngineBuilder()
			.UseFileSystemProject(root)
			.UseMemoryCachingProvider()
			.Build();
	}

	public async Task<string> RenderAsync<T>(string templateName, T model)
	{
		return await _engine.CompileRenderAsync(templateName, model);
	}
}
