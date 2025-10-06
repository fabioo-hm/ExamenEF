using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnType("varchar(60)");

        builder.Property(c => c.Email)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(c => c.Phone)
            .HasColumnType("varchar(20)");

        builder.HasMany(c => c.Vehicles)
               .WithOne(v => v.Customer)
               .HasForeignKey(v => v.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}