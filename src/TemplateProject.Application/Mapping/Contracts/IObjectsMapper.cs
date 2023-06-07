using System;
using System.Collections.Generic;
using TemplateProject.Application.Profile.Models;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.Application.Mapping.Contracts;

public interface IObjectsMapper
{
    CurrentUserProfileResponse ToCurrentUserProfileResponse(User user);

    IReadOnlyCollection<TDestination> MapCollection<TSource, TDestination>(
        IEnumerable<TSource> sources,
        Func<TSource, TDestination> rule);
}