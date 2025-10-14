using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Auth;

namespace Domain.Entities;

public class Auditoria
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string EntidadAfectada { get; private set; } = null!;
    public string Accion { get; private set; } = null!;
    public DateTime FechaHora { get; private set; }
    public string Detalles { get; private set; } = null!;
     public int RegistroAfectadoId { get; private set; }

    public int UserMemberId { get; private set; }
    public virtual UserMember? UserMember { get; set; }

    private Auditoria() { }

    public Auditoria(
        string entidadAfectada,
        string accion,
        string detalles,
        DateTime fechaHora,
        int userMemberId,
        int registroAfectadoId)
    {
        EntidadAfectada = entidadAfectada;
        Accion = accion;
        Detalles = detalles;
        FechaHora = fechaHora;
        UserMemberId = userMemberId;
        RegistroAfectadoId = registroAfectadoId;
    }
}