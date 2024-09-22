using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TemplateProject.Core.Contracts.Repositories;
using TemplateProject.DataAccess.Connection;

namespace TemplateProject.DataAccess.Repositories;

public sealed class UnitOfWork : IUnitOfWork, IDisposable
{
	private readonly DatabaseContext _databaseContext;
	private readonly ILogger<UnitOfWork> _logger;
	private readonly IServiceScope _serviceScope;

	private IUserRepository _userRepository;
	private IRefreshTokenRepository _refreshTokenRepository;

	public UnitOfWork(IServiceProvider serviceProvider)
    {
	    _serviceScope = serviceProvider.CreateScope();
	    _logger = GetService<ILogger<UnitOfWork>>();
	    _databaseContext = GetService<DatabaseContext>();
    }
	
	public IUserRepository Users => _userRepository ??= GetService<IUserRepository>();
	public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= GetService<IRefreshTokenRepository>();

	public async Task CommitTransactionAsync(Action action)
	{
		await using var transaction = await _databaseContext.Database.BeginTransactionAsync();

		try
		{
			action();
			await _databaseContext.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Exception occured in transaction: {Message}", exception.Message);
			await transaction.RollbackAsync();
			throw;
		}
	}

	public async Task CommitTransactionAsync(Func<Task> action)
	{
		await using var transaction = await _databaseContext.Database.BeginTransactionAsync();

		try
		{
			await action();
			await _databaseContext.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Exception occured in transaction: {Message}", exception.Message);
			await transaction.RollbackAsync();
			throw;
		}
	}
    
    public async Task<TResult> CommitTransactionAsync<TResult>(Func<TResult> action)
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync();

        try
        {
            var result = action();
            await _databaseContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occured in transaction: {Message}", exception.Message);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task CommitAsync()
    {
	    await _databaseContext.SaveChangesAsync();
    }

    public void Dispose()
    {
	    _serviceScope?.Dispose();
    }

    private TInterface GetService<TInterface>()
    {
	    return _serviceScope.ServiceProvider.GetRequiredService<TInterface>();
    }
}
