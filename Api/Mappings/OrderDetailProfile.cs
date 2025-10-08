using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.OrderDetails;
using AutoMapper;
using Domain.Entities;

namespace Api.Mappings;

public class OrderDetailProfile : Profile
{
    public OrderDetailProfile()
    {
        CreateMap<OrderDetail, OrderDetailDto>();

        CreateMap<CreateOrderDetailDto, OrderDetail>()
            .ConstructUsing(src => new OrderDetail(
                src.ServiceOrderId,
                src.SparePartId,
                src.Quantity,
                src.UnitCost
            ));
        
        CreateMap<UpdateOrderDetailDto, OrderDetail>()
            .ConstructUsing(src => new OrderDetail(
                src.ServiceOrderId,
                src.SparePartId,
                src.Quantity,
                src.UnitCost
            ));
    }
}
