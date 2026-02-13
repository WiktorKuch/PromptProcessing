using MassTransit;
using Microsoft.EntityFrameworkCore;
using PromptApi.Domain;
using PromptApi.Infrastructure;
using PromptApi.Services;

namespace PromptApi.Consumers;

public class PromptConsumer : IConsumer<ProcessPrompt>
{
    private readonly AppDbContext _db;
    private readonly IOpenAiService _openAiService;
    private readonly ILogger<PromptConsumer> _logger;

    public PromptConsumer(
        AppDbContext db,
        IOpenAiService openAiService,
        ILogger<PromptConsumer> logger)
    {
        _db = db;
        _openAiService = openAiService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPrompt> context)
    {
        _logger.LogInformation("Processing prompt {PromptId}", context.Message.PromptId);

        var prompt = await _db.Prompts.FindAsync(context.Message.PromptId);
        if (prompt == null)
        {
            _logger.LogWarning("Prompt {PromptId} not found", context.Message.PromptId);
            return;
        }

        try
        {
            prompt.StartProcessing();
            await _db.SaveChangesAsync(context.CancellationToken);

            var result = await _openAiService.ProcessAsync(context.Message.Content, context.CancellationToken);
            prompt.Complete(result);
            await _db.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("Prompt {PromptId} completed", context.Message.PromptId);
        }
        catch (Exception ex)
        {
            prompt.Fail(ex.Message);
            await _db.SaveChangesAsync(context.CancellationToken);
            _logger.LogError(ex, "Failed to process prompt {PromptId}", context.Message.PromptId);
            throw;
        }
    }
}
