using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;

namespace Api.DTOs.ServiceOrders;

public record CreateServiceOrderDto( Guid VehicleId, ServiceType ServiceType, int UserMemberId, DateTime EntryDate, DateTime EstimatedDeliveryDate, OrderStatus OrderStatus);