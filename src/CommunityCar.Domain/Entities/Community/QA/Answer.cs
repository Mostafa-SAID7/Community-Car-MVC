using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.QA;

public class Answer : BaseEntity
{
    public string Body { get; private set; }
    
    // Arabic Localization
    public string? BodyAr { get; private set; }
    
    public Guid QuestionId { get; private set; }
    public Guid AuthorId { get; private set; }
    public bool IsAccepted { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    
    // Additional properties for enhanced answers
    public int VoteScore { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTime? LastEditedAt { get; private set; }
    public string? EditReason { get; private set; }
    public int HelpfulCount { get; private set; }
    
    // Quality indicators
    public bool IsVerifiedByExpert { get; private set; }
    public Guid? VerifiedBy { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? VerificationNote { get; private set; }

    public Answer(string body, Guid questionId, Guid authorId)
    {
        Body = body;
        QuestionId = questionId;
        AuthorId = authorId;
        IsAccepted = false;
        VoteScore = 0;
        IsEdited = false;
        HelpfulCount = 0;
        IsVerifiedByExpert = false;
    }

    private Answer() { }

    public void Accept()
    {
        IsAccepted = true;
        AcceptedAt = DateTime.UtcNow;
        Audit(UpdatedBy);
    }

    public void Unaccept()
    {
        IsAccepted = false;
        AcceptedAt = null;
        Audit(UpdatedBy);
    }

    public void UpdateBody(string newBody, string? editReason = null)
    {
        Body = newBody;
        IsEdited = true;
        LastEditedAt = DateTime.UtcNow;
        EditReason = editReason;
        Audit(UpdatedBy);
    }

    public void UpdateArabicContent(string? bodyAr)
    {
        BodyAr = bodyAr;
        Audit(UpdatedBy);
    }

    public void UpdateVoteScore(int score)
    {
        VoteScore = score;
    }

    public void IncrementHelpfulCount()
    {
        HelpfulCount++;
    }

    public void DecrementHelpfulCount()
    {
        if (HelpfulCount > 0)
            HelpfulCount--;
    }

    public void VerifyByExpert(Guid expertId, string? note = null)
    {
        IsVerifiedByExpert = true;
        VerifiedBy = expertId;
        VerifiedAt = DateTime.UtcNow;
        VerificationNote = note;
        Audit(UpdatedBy);
    }

    public void RemoveExpertVerification()
    {
        IsVerifiedByExpert = false;
        VerifiedBy = null;
        VerifiedAt = null;
        VerificationNote = null;
        Audit(UpdatedBy);
    }
}
