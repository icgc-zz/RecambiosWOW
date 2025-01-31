namespace RecambiosWOW.Core.Domain.ValueObjects;

public record Answer
{
    public string Text { get; }
    public DateTime CreatedAt { get; }

    public Answer(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Answer text cannot be empty", nameof(text));

        Text = text;
        CreatedAt = DateTime.UtcNow;
    }
}