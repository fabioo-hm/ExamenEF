using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Auth;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasColumnType("varchar(200)");

        builder.Property(rt => rt.Expires)
            .IsRequired()
            .HasColumnType("timestamp");

        builder.Property(rt => rt.Created)
            .IsRequired()
            .HasColumnType("timestamp");

        builder.Property(rt => rt.Revoked)
            .HasColumnType("timestamp");

        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsActive);

        builder.HasOne(rt => rt.UserMember)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}