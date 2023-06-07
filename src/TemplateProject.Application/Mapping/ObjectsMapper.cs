using System;
using System.Collections.Generic;
using System.Linq;
using TemplateProject.Application.Mapping.Contracts;
using TemplateProject.Application.Profile.Models;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.Application.Mapping;

public sealed class ObjectsMapper : IObjectsMapper
{
    public CurrentUserProfileResponse ToCurrentUserProfileResponse(User user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        return new CurrentUserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Fullname = user.Fullname,
        };
    }

    public IReadOnlyCollection<TDestination> MapCollection<TSource, TDestination>(
        IEnumerable<TSource> sources,
        Func<TSource, TDestination> rule)
    {
        return sources?.Select(rule).ToArray();
    }
}