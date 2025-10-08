using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Vehicles;
using AutoMapper;
using Domain.Entities;

namespace Api.Mappings;

public class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        CreateMap<Vehicle, VehicleDto>();

        CreateMap<CreateVehicleDto, Vehicle>()
            .ConstructUsing(src => new Vehicle(
                src.Brand!,
                src.Model!,
                src.Year,
                src.Vin!,
                src.Mileage,
                src.CustomerId
            ));

        CreateMap<UpdateVehicleDto, Vehicle>()
            .ConstructUsing(src => new Vehicle(
                src.Brand!,
                src.Model!,
                src.Year,
                src.Vin!,
                src.Mileage,
                src.CustomerId
            ));
    }
}
