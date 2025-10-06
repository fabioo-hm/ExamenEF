using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class SparePartConfiguration : IEntityTypeConfiguration<SparePart>
{
    public void Configure(EntityTypeBuilder<SparePart> builder)
    {
        builder.ToTable("spare_parts");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .HasColumnType("varchar(255)")
            .HasMaxLength(255);

        builder.Property(s => s.StockQuantity)
            .IsRequired()
            .HasColumnType("integer");

        builder.Property(s => s.UnitPrice)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.HasMany(s => s.OrderDetails)
            .WithOne(od => od.SparePart)
            .HasForeignKey(od => od.SparePartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.Code).IsUnique();
    }
}