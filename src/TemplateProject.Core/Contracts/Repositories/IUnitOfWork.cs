using System;
using System.Threading.Tasks;

namespace TemplateProject.Core.Contracts.Repositories;

public interface IUnitOfWork
{
    IRefreshTokenRepository RefreshTokens { get; }
    IUserRepository Users { get; }

    public Task CommitTransactionAsync(Action action);
    public Task CommitTransactionAsync(Func<Task> action);
    public Task<TResult> CommitTransactionAsync<TResult>(Func<TResult> action);
    public Task CommitAsync();
}