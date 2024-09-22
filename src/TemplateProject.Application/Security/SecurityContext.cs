using System;

namespace TemplateProject.Application.Security;

public sealed class SecurityContext : ISecurityContext
{
    public Guid UserId { get; private set; }
    public bool IsAuthenticated { get; private set; }
    public bool IsInitialized { get; private set; }
    
    public void Initialize(Guid userId)
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException("Security context is already initialized.");
        }

        UserId = userId;
        IsAuthenticated = true;
        IsInitialized = true;
    }

    public void InitializeAsAnonymous()
    {
        IsAuthenticated = false;
        IsInitialized = true;
    }
}