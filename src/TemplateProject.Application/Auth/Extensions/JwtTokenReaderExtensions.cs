using System;
using Kirpichyov.FriendlyJwt.Contracts;

namespace TemplateProject.Application.Auth.Extensions;

public static class JwtTokenReaderExtensions
{
    public static Guid GetUserId(this IJwtTokenReader jwtTokenReader)
    {
        return Guid.Parse(jwtTokenReader.UserId);
    }
}