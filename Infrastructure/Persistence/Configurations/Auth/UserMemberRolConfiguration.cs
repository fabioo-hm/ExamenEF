using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Auth;

public sealed class UserMemberRolConfiguration : IEntityTypeConfiguration<UserMemberRol>
{
    public void Configure(EntityTypeBuilder<UserMemberRol> builder)
    {
        builder.ToTable("user_member_roles");

        builder.HasKey(ur => new { ur.UserMemberId, ur.RolId });

        builder.HasOne(ur => ur.UserMembers)
            .WithMany(u => u.UserMemberRols)
            .HasForeignKey(ur => ur.UserMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Rol)
            .WithMany(r => r.UserMemberRols)
            .HasForeignKey(ur => ur.RolId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}