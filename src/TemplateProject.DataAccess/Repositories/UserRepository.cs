using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Core.Contracts.Repositories;
using TemplateProject.Core.Models.Entities;
using TemplateProject.DataAccess.Connection;
using TemplateProject.DataAccess.Extensions;

namespace TemplateProject.DataAccess.Repositories;

public sealed class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(DatabaseContext context)
        : base(context)
    {
    }

    public async Task<User> TryGet(Guid id, bool withTracking)
    {
        return await Context.Users
            .WithTracking(withTracking)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User> TryGet(string email, bool withTracking)
    {
        return await Context.Users
            .WithTracking(withTracking)
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> IsEmailExists(string email)
    {
        return await Context.Users.AnyAsync(user => user.Email == email);
    }
}