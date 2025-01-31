namespace RecambiosWOW.Core.Domain.ValueObjects;

public class ListingMetrics
{
    public int Views { get; private set; }
    public int SaveCount { get; private set; }
    public int QuestionCount { get; private set; }
    public int ContactCount { get; private set; }
    public DateTime LastViewed { get; private set; }
    public Dictionary<string, int> SourceViews { get; private set; }

    public ListingMetrics()
    {
        Views = 0;
        SaveCount = 0;
        QuestionCount = 0;
        ContactCount = 0;
        LastViewed = DateTime.UtcNow;
        SourceViews = new Dictionary<string, int>();
    }

    public void IncrementViews(string? source = null)
    {
        Views++;
        LastViewed = DateTime.UtcNow;
        
        if (!string.IsNullOrWhiteSpace(source))
        {
            if (!SourceViews.ContainsKey(source))
                SourceViews[source] = 0;
            SourceViews[source]++;
        }
    }

    public void IncrementSaves()
    {
        SaveCount++;
    }

    public void IncrementQuestions()
    {
        QuestionCount++;
    }

    public void IncrementContacts()
    {
        ContactCount++;
    }
}