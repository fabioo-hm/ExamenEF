using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Auditorias;
using AutoMapper;
using Domain.Entities;

namespace Api.Mappings;
public class AuditoriaProfile : Profile
{
    public AuditoriaProfile()
    {
        CreateMap<Auditoria, AuditoriaDto>();

        CreateMap<CreateAuditoriaDto, Auditoria>()
            .ConstructUsing(src => new Auditoria(
                src.EntidadAfectada, 
                src.Accion, 
                src.Detalles, 
                src.FechaHora, 
                src.RegistroAfectadoId, 
                src.UserMemberId
            ));
    }
}