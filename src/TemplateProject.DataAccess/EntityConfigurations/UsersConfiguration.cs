using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.DataAccess.EntityConfigurations;

public sealed class UsersConfiguration : EntityConfigurationBase<User, Guid>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(entity => entity.Firstname).IsRequired();
        builder.Property(entity => entity.Lastname).IsRequired();
        builder.Property(entity => entity.Email).IsRequired();
        builder.Property(entity => entity.PasswordHash).IsRequired();
    }
}