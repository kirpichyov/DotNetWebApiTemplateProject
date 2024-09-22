using System.Collections.Generic;

namespace TemplateProject.Core.Contracts.Repositories;

public interface IRepositoryBase<in TEntity> 
    where TEntity : class
{
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}
