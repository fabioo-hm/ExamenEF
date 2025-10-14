using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Abstractions;
public sealed record CreateAuditoria(string EntidadAfectada, string Accion, DateTime FechaHora, string Detalles, int RegistroAfectadoId, int UserMemberId) : IRequest<Guid>;