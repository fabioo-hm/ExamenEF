using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Auth;

public sealed class UserMemberConfiguration : IEntityTypeConfiguration<UserMember>
{
    public void Configure(EntityTypeBuilder<UserMember> builder)
    {
        builder.ToTable("user_members");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .ValueGeneratedOnAdd()
               .IsRequired()
               .HasColumnName("id");

        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("name");

        builder.Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnName("email");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Password)
           .HasColumnType("varchar")
           .HasMaxLength(255)
           .IsRequired();

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.UserMember)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserMemberRols)
            .WithOne(ur => ur.UserMembers)
            .HasForeignKey(ur => ur.UserMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Auditorias)
            .WithOne(a => a.UserMember)
            .HasForeignKey(a => a.UserMemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
