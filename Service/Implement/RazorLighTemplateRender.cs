using RazorLight;

namespace ScalarDemo.Service.Implement;

public class RazorLighTemplateRender : ITemplateService
{
	private readonly RazorLightEngine _engine;
	public RazorLighTemplateRender()
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
