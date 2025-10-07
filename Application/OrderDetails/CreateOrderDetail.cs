using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.OrderDetails;

public sealed record CreateCompany(int Quantity, decimal UnitCost, decimal  Subtotal,Guid SparePartId, Guid ServiceOrderId) : IRequest<Guid>;