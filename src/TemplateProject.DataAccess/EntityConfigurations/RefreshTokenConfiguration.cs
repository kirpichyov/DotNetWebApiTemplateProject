using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.DataAccess.EntityConfigurations;

public sealed class RefreshTokenConfiguration : EntityConfigurationBase<RefreshToken, Guid>
{
    public override void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        base.Configure(builder);

        builder.Property(entity => entity.IsInvalidated).IsRequired();
        builder.Property(entity => entity.AccessTokenId).IsRequired();
        builder.Property(entity => entity.CreatedAtUtc).IsRequired();

        builder.HasOne(entity => entity.User)
            .WithMany()
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}