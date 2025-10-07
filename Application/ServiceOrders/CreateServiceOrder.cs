using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.ServiceOrders;
public sealed record CreateBranch(string MechanicAssigned,DateTime EntryDate, DateTime EstimatedDeliveryDate, Guid VehicleId) : IRequest<Guid>;