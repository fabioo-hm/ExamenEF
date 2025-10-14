using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos.Auditorias;
public record CreateAuditoriaDto(string EntidadAfectada, string Accion, DateTime FechaHora, string Detalles, int RegistroAfectadoId, int UserMemberId);