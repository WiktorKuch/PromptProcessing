using Microsoft.EntityFrameworkCore;
using PromptApi.Domain;
using PromptApi.Infrastructure;
using PromptApi.Services;

namespace PromptApi.Workers;

public class PromptProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PromptProcessor> _logger;
    

    public PromptProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<PromptProcessor> logger
        )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PromptProcessor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var openAiService = scope.ServiceProvider.GetRequiredService<IOpenAiService>(); 

            var prompt = await db.Prompts
                .Where(p => p.Status == PromptStatus.Pending)
                .OrderBy(p => p.CreatedAt)
                .FirstOrDefaultAsync(stoppingToken);

            if (prompt == null)
            {
                await Task.Delay(2000, stoppingToken);
                continue;
            }

            try
            {
                prompt.Status = PromptStatus.Processing;
                await db.SaveChangesAsync(stoppingToken);

                var result = await openAiService.ProcessAsync(prompt.Content, stoppingToken); 
                prompt.Result = result;
                
                prompt.Status = PromptStatus.Completed;
                await db.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Prompt {PromptId} completed", prompt.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR OpenAI for prompt {PromptId}: {Message}", prompt.Id, ex.Message);
                prompt.Status = PromptStatus.Failed;
                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
