using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Auditorias;
public sealed class CreateAuditoriaHandler(IAuditoriaRepository repo) : IRequestHandler<CreateAuditoria, Guid>
{
    public async Task<Guid> Handle(CreateAuditoria req, CancellationToken ct)
    {
        var auditoria = new Auditoria(req.EntidadAfectada, req.Accion, req.Detalles, req.FechaHora, req.RegistroAfectadoId, req.UserMemberId);
        await repo.AddAsync(auditoria, ct);
        return auditoria.Id;
    }
}