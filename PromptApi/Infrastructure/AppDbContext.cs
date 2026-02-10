using Microsoft.EntityFrameworkCore;
using PromptApi.Domain;

namespace PromptApi.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Prompt> Prompts => Set<Prompt>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
