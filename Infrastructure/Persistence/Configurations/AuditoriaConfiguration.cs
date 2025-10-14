using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        builder.ToTable("auditorias");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntidadAfectada)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Accion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.FechaHora)
            .IsRequired();

        builder.Property(a => a.Detalles)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.RegistroAfectadoId)
            .IsRequired();

        builder.HasOne(a => a.UserMember)
            .WithMany(u => u.Auditorias)
            .HasForeignKey(a => a.UserMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.FechaHora);
        builder.HasIndex(a => a.EntidadAfectada);
    }
}
