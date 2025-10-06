using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.ToTable("service_orders");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.MechanicAssigned)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(s => s.EntryDate)
            .IsRequired()
            .HasColumnType("timestamp");

        builder.Property(s => s.EstimatedDeliveryDate)
            .IsRequired()
            .HasColumnType("timestamp");

        builder.Property(s => s.ServiceType)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),                             // Enum -> string
                v => (ServiceType)Enum.Parse(typeof(ServiceType), v)) // string -> Enum
            .HasColumnType("varchar(50)");

        builder.HasOne(s => s.Vehicle)
               .WithMany(v => v.ServiceOrders)
               .HasForeignKey(s => s.VehicleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.OrderDetails)
               .WithOne(d => d.ServiceOrder)
               .HasForeignKey(d => d.ServiceOrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Invoice)
               .WithOne(i => i.ServiceOrder)
               .HasForeignKey<Invoice>(i => i.ServiceOrderId)
               .OnDelete(DeleteBehavior.Cascade);

    }
}