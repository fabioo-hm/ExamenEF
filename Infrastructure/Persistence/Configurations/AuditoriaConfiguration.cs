using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;
public class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> builder)
    {
        // Nombre de la tabla
        builder.ToTable("Auditorias");

        // Clave primaria
        builder.HasKey(a => a.Id);

        // Propiedades
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

        // Relación con UserMember
        builder.HasOne(a => a.UserMember)
            .WithMany() // Si UserMember no tiene lista de auditorías
            .HasForeignKey(a => a.UserMemberId)
            .OnDelete(DeleteBehavior.Restrict); // Evita borrado en cascada

        // Índices
        builder.HasIndex(a => a.FechaHora);
        builder.HasIndex(a => a.EntidadAfectada);
    }
}