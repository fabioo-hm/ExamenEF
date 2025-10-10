using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;
using MediatR;

namespace Application.ServiceOrders;

public sealed record CreateServiceOrder( Guid VehicleId, ServiceType ServiceType, int UserMemberId, DateTime EntryDate, DateTime EstimatedDeliveryDate, OrderStatus OrderStatus) : IRequest<Guid>;