using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Brand)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(v => v.Model)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(v => v.Year)
            .IsRequired()
            .HasColumnType("integer");

        builder.Property(v => v.Vin)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(v => v.Mileage)
            .IsRequired()
            .HasColumnType("double precision");

        builder.HasOne(v => v.Customer)
               .WithMany(c => c.Vehicles)
               .HasForeignKey(v => v.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.Vin)
               .IsUnique();

        builder.HasIndex(v => v.Brand);
        builder.HasIndex(v => v.Model);
    }
}