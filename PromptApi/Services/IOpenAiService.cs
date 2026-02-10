namespace PromptApi.Services;

public interface IOpenAiService
{
    Task<string> ProcessAsync(string prompt, CancellationToken cancellationToken);
}
