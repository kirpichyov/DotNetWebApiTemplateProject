using System;

namespace TemplateProject.Core.Models.Entities;

public sealed class RefreshToken : EntityBase<Guid>
{
    public RefreshToken(Guid accessTokenId, DateTime createdAtUtc, User user)
        : base(Guid.NewGuid())
    {
        AccessTokenId = accessTokenId;
        CreatedAtUtc = createdAtUtc;

        UserId = user.Id;
        User = user;
    }

    private RefreshToken()
    {
    }

    public Guid AccessTokenId { get; }
    public DateTime CreatedAtUtc { get; }
    public bool IsInvalidated { get; private set; }

    public Guid UserId { get; }
    public User User { get; }
    
    public void Invalidate()
    {
        if (IsInvalidated)
        {
            throw new InvalidOperationException("Refresh token is already invalidated.");
        }
        
        IsInvalidated = true;
    }
}
