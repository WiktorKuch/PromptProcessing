namespace PromptApi.Domain;

public class Prompt
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public PromptStatus Status { get; set; }
    public string? Result { get; set; }
    public DateTime CreatedAt { get; set; }
}
