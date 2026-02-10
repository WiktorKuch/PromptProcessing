using Microsoft.AspNetCore.Mvc;
using PromptApi.Domain;
using PromptApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace PromptApi.Controllers;

[ApiController]
[Route("api/prompts")]
public class PromptsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PromptsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] string content)
    {
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Content = content,
            Status = PromptStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Prompts.Add(prompt);
        await _db.SaveChangesAsync();

        return Ok(prompt);
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
