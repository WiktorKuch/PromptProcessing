namespace PromptApi.Domain;

public class Prompt
{
    public Guid Id { get; private set; }
    public string Content { get; private set; } = null!;
    public PromptStatus Status { get; private set; }
    public string? Result { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Prompt(string content)
    {
        Id = Guid.NewGuid();
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Status = PromptStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    public void StartProcessing()
    {
        if (Status != PromptStatus.Pending)
            throw new DomainException("Only pending prompts can be processed");
        Status = PromptStatus.Processing;
    }

    public void Complete(string result)
    {
        if (Status != PromptStatus.Processing)
            throw new DomainException("Only processing prompts can be completed");
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Status = PromptStatus.Completed;
    }

    public void Fail(string? error = null)
    {
        if (Status == PromptStatus.Completed)
            throw new DomainException("Completed prompts cannot fail");
        Status = PromptStatus.Failed;
        Result = error;
    }
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
