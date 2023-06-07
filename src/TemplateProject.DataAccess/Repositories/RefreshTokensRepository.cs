using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Core.Models.Entities;
using TemplateProject.DataAccess.Connection;
using TemplateProject.DataAccess.Contracts;
using TemplateProject.DataAccess.Extensions;

namespace TemplateProject.DataAccess.Repositories;

public sealed class RefreshTokensRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokensRepository(DatabaseContext context)
        : base(context)
    {
    }

    public async Task<RefreshToken> FindById(Guid id, bool useTracking)
    {
        return await Context.RefreshTokens
            .WithTracking(useTracking).FirstOrDefaultAsync(token => token.Id == id);
    }

    public async Task<IReadOnlyCollection<RefreshToken>> FindAllByUserId(Guid userId, bool useTracking)
    {
        return await Context.RefreshTokens
            .WithTracking(useTracking)
            .Where(token => token.UserId == userId)
            .ToArrayAsync();
    }
}