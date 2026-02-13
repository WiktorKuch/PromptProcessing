using Microsoft.AspNetCore.Mvc;
using MassTransit;
using PromptApi.Domain;
using PromptApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace PromptApi.Controllers;

[ApiController]
[Route("api/prompts")]
public class PromptsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPublishEndpoint _publishEndpoint;

    public PromptsController(AppDbContext db, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string content)
    {
        var prompt = new Prompt(content);

        _db.Prompts.Add(prompt);
        await _db.SaveChangesAsync();
        
        await _publishEndpoint.Publish(new ProcessPrompt(prompt.Id, content));

        return Accepted(prompt);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var prompts = await _db.Prompts
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(prompts);
    }
}
