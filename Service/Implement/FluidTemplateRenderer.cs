using System.Globalization;
using Fluid;
using Fluid.Values;
using ScalarDemo.Model;

namespace ScalarDemo.Service.Implement;

public class FluidTemplateRenderer : ITemplateService
{
	private readonly FluidParser _parser = new();
	private readonly TemplateOptions _options = new();

	public FluidTemplateRenderer()
	{

		_options.MemberAccessStrategy.Register<WelcomeEmailModel>();
		_options.CultureInfo = new CultureInfo("en-US");
		_options.Filters.AddFilter("currency", (input, args, ctx) =>
		{
			var value = Convert.ToDecimal(input.ToStringValue());
			return new StringValue(value.ToString("#,##0") + " ₫");
		});
	}

	public async Task<string> RenderAsync<T>(string template, T model)
	{
		if (!_parser.TryParse(template, out var fluidTemplate, out var errors))
		{
			throw new Exception(string.Join("\n", errors));
		}

		var context = new TemplateContext(model, _options);
		return await fluidTemplate.RenderAsync(context);
	}
}
