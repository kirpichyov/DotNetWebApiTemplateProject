using System;

namespace TemplateProject.Application.Security;

public interface ISecurityContext
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
    bool IsInitialized { get; }
}