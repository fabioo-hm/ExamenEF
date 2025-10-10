using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.ServiceOrders;
using AutoMapper;
using Domain.Entities;

namespace Api.Mappings;

public class ServiceOrderProfile : Profile
{
    public ServiceOrderProfile()
    {
        CreateMap<ServiceOrder, ServiceOrderDto>();

        CreateMap<CreateServiceOrderDto, ServiceOrder>()
            .ConstructUsing(src => new ServiceOrder(
                src.VehicleId,
                src.ServiceType,
                src.UserMemberId,
                src.EntryDate,
                src.EstimatedDeliveryDate,
                src.OrderStatus
            ));
        CreateMap<UpdateServiceOrderDto, ServiceOrder>()
            .ConstructUsing(src => new ServiceOrder(
                src.VehicleId,
                src.ServiceType,
                src.UserMemberId,
                src.EntryDate,
                src.EstimatedDeliveryDate,
                src.OrderStatus
            ));
    }
}
