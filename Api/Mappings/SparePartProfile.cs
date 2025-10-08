using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.SpareParts;
using AutoMapper;
using Domain.Entities;

namespace Api.Mappings;

public class SparePartProfile : Profile
{
    public SparePartProfile()
    {
        CreateMap<SparePart, SparePartDto>();

        CreateMap<CreateSparePartDto, SparePart>()
            .ConstructUsing(src => new SparePart(
                src.Code!,
                src.Description!,
                src.StockQuantity,
                src.UnitPrice
            ));
        
        CreateMap<UpdateSparePartDto, SparePart>()
            .ConstructUsing(src => new SparePart(
                src.Code!,
                src.Description!,
                src.StockQuantity,
                src.UnitPrice
            ));
    }
}
