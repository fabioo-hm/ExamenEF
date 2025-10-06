using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Auth;

public sealed class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);
        builder.Property(di => di.Id)
                .ValueGeneratedOnAdd()
                .IsRequired()
                .HasColumnName("id");

        builder.Property(p => p.Name)
        .HasColumnName("rolName")
        .HasColumnType("varchar")
        .HasMaxLength(50)
        .IsRequired();

        builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(50);

        builder.HasMany(r => r.UserMemberRols)
            .WithOne(ur => ur.Rol)
            .HasForeignKey(ur => ur.RolId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}