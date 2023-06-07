﻿using System;
using System.Threading.Tasks;
using Kirpichyov.FriendlyJwt.Contracts;
using TemplateProject.Application.Auth.Extensions;
using TemplateProject.Application.Mapping.Contracts;
using TemplateProject.Application.Profile.Contracts;
using TemplateProject.Application.Profile.Models;
using TemplateProject.DataAccess.Contracts;

namespace TemplateProject.Application.Profile;

public sealed class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectsMapper _objectsMapper;
    private readonly IJwtTokenReader _jwtTokenReader;

    public ProfileService(
        IUnitOfWork unitOfWork,
        IObjectsMapper objectsMapper,
        IJwtTokenReader jwtTokenReader)
    {
        _unitOfWork = unitOfWork;
        _objectsMapper = objectsMapper;
        _jwtTokenReader = jwtTokenReader;
    }

    public async Task<CurrentUserProfileResponse> GetCurrentProfile()
    {
        if (!_jwtTokenReader.IsLoggedIn)
        {
            throw new InvalidOperationException("Authorization is required for this operation");
        }

        var currentUserId = _jwtTokenReader.GetUserId();
        var currentUser = await _unitOfWork.Users.TryGet(currentUserId, withTracking: false);

        return _objectsMapper.ToCurrentUserProfileResponse(currentUser);
    }
}