using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.OrderDetails;

public sealed class CreateOrderDetailHandler(IOrderDetailRepository repo) : IRequestHandler<CreateOrderDetail, Guid>
{
    public async Task<Guid> Handle(CreateOrderDetail req, CancellationToken ct)
    {
        var orderDetail = new OrderDetail(req.ServiceOrderId, req.SparePartId, req.Quantity, req.UnitCost
        );

        await repo.AddAsync(orderDetail, ct);
        return orderDetail.Id;
    }
}