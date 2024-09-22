using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kirpichyov.FriendlyJwt.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TemplateProject.Core.Contracts;

namespace TemplateProject.DataAccess.Interceptors;

public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    public AuditableEntitySaveChangesInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    
    private void UpdateAuditableEntities(DbContext dbContext)
    {
        using var scope = _serviceProvider.CreateScope();
        var jwtTokenReader = scope.ServiceProvider.GetRequiredService<IJwtTokenReader>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
        
        var entries = dbContext.ChangeTracker.Entries()
            .Where(e => e is { Entity: IAuditEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entry in entries)
        {
            var entity = (IAuditEntity)entry.Entity;
            var userEmail = jwtTokenReader.UserEmail;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAtUtc = dateTimeProvider.UtcNow;
                    entity.CreatedBy = userEmail;
                    break;
                case EntityState.Modified:
                    entity.UpdatedAtUtc = dateTimeProvider.UtcNow;
                    entity.UpdatedBy = userEmail;
                    break;
            }
        }
    }
}