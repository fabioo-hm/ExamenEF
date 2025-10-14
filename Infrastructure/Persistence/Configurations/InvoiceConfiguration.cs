using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.IssueDate)
            .IsRequired()
            .HasColumnType("timestamp");

        builder.Property(i => i.LaborCost)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(i => i.PartsTotal)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Ignore(i => i.Total);

        builder.HasOne(i => i.ServiceOrder)
            .WithOne(so => so.Invoice)
            .HasForeignKey<Invoice>(i => i.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(s => s.PaymentMethod)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),                             
                v => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), v)) 
            .HasColumnType("varchar(50)");
    }
}