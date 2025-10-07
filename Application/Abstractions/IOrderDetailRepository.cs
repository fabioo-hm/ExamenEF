using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IOrderDetailRepository
{
    Task<OrderDetail?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<OrderDetail>> GetByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct = default);
    Task<IReadOnlyList<OrderDetail>> GetBySparePartIdAsync(Guid sparePartId, CancellationToken ct = default);
    Task<IReadOnlyList<OrderDetail>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(OrderDetail orderDetail, CancellationToken ct = default);
    Task UpdateAsync(OrderDetail orderDetail, CancellationToken ct = default);
    Task RemoveAsync(OrderDetail orderDetail, CancellationToken ct = default);

    Task<decimal> GetTotalCostByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid serviceOrderId, Guid sparePartId, CancellationToken ct = default);
}
