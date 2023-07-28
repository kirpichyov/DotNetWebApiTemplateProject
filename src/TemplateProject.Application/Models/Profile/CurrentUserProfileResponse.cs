using System;

namespace TemplateProject.Application.Models.Profile;

public sealed record CurrentUserProfileResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required string Fullname { get; init; }
}