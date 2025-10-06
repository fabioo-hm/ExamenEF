using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("order_details");
        builder.HasKey(od => od.Id);

        builder.Property(od => od.Quantity)
            .IsRequired()
            .HasColumnType("integer");

        builder.Property(od => od.UnitCost)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Ignore(od => od.Subtotal);

        builder.HasOne(od => od.ServiceOrder)
            .WithMany(so => so.OrderDetails)
            .HasForeignKey(od => od.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(od => od.SparePart)
            .WithMany(sp => sp.OrderDetails)
            .HasForeignKey(od => od.SparePartId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}