using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;

namespace Application.DTOs.ServiceOrders;

public class UpdateServiceOrderStatusDto
{
    public OrderStatus OrderStatus { get; set; }
}