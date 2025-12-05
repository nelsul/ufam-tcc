namespace IcompCare.Application.Interfaces;

public interface IEmailTemplateService
{
    string RenderTemplate(string templateName, object model);
    string GetBaseLayout(string content, string title);
}
