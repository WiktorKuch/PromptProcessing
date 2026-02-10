using OpenAI.Chat;

namespace PromptApi.Services;

public class OpenAiService : IOpenAiService
{
    private readonly ChatClient _client;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(IConfiguration configuration, ILogger<OpenAiService> logger)
    {
        _logger = logger;

        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                 ?? configuration["OpenAI:ApiKey"]
                 ?? throw new InvalidOperationException("OpenAI API key missing");

        var model = Environment.GetEnvironmentVariable("OPENAI_MODEL")
                    ?? configuration["OpenAI:Model"]
                    ?? "gpt-4o-mini";

        _client = new ChatClient(model, apiKey);
    }

    public async Task<string> ProcessAsync(string prompt, CancellationToken cancellationToken = default)
    {

        if (prompt.Trim().ToLower() == "error test")
        {
            _logger.LogWarning("TEST ERROR - simulated OpenAI failure");
            throw new Exception("Simulated OpenAI processing failure");
        }


        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("Prompt cannot be empty", nameof(prompt));

        try
        {
            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompt)
            };

            var response = await _client.CompleteChatAsync(messages, cancellationToken: cancellationToken);

            return response.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing prompt: {Prompt}", prompt);
            throw;
        }
    }
}
