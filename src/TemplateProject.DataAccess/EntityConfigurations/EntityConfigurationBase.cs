using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.DataAccess.EntityConfigurations;

public abstract class EntityConfigurationBase<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityBase<TId>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(entity => entity.Id);
    }
}
