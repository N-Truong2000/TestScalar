namespace ScalarDemo.Service;

public interface ITemplateService
{
	Task<string> RenderAsync<T>(string templateName, T model);
}
