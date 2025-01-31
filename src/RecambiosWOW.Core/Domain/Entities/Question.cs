using RecambiosWOW.Core.Domain.Enums;
using RecambiosWOW.Core.Domain.ValueObjects;

namespace RecambiosWOW.Core.Domain.Entities;

public class Question
{
    public int Id { get; private set; }
    public PartListing Listing { get; private set; }
    public Member AskedBy { get; private set; }
    public string Text { get; private set; }
    public Answer? Answer { get; private set; }
    public QuestionStatus Status { get; private set; }
    public AuditInfo AuditInfo { get; private set; }

    public Question(
        PartListing listing,
        Member askedBy,
        string text)
    {
        Listing = listing ?? throw new ArgumentNullException(nameof(listing));
        AskedBy = askedBy ?? throw new ArgumentNullException(nameof(askedBy));
        
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Question text cannot be empty", nameof(text));

        Text = text;
        Status = QuestionStatus.Pending;
        AuditInfo = new AuditInfo(DateTime.UtcNow);
    }

    public void AddAnswer(string text)
    {
        if (Answer != null)
            throw new InvalidOperationException("Question is already answered");

        Answer = new Answer(text);
        Status = QuestionStatus.Answered;
        AuditInfo = AuditInfo.Update();
    }

    protected Question() { }
}