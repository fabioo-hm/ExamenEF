using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.ServiceOrders;
public sealed class CreateServiceOrderHandler(IServiceOrderRepository repo) : IRequestHandler<CreateServiceOrder, Guid>
{
    public async Task<Guid> Handle(CreateServiceOrder req, CancellationToken ct)
    {
        var serviceOrder = new ServiceOrder(req.VehicleId, req.ServiceType, req.MechanicAssigned, req.EntryDate, req.EstimatedDeliveryDate);
        await repo.AddAsync(serviceOrder, ct);
        return serviceOrder.Id;
    }
}