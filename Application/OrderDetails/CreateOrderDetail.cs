using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.OrderDetails;

public sealed record CreateOrderDetail(Guid ServiceOrderId, Guid SparePartId, int Quantity, decimal UnitCost) : IRequest<Guid>;