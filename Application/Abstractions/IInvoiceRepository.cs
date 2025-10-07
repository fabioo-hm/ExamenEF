using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Abstractions;
public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Invoice?> GetByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByPaymentMethodAsync(PaymentMethod method, CancellationToken ct = default);

    Task AddAsync(Invoice invoice, CancellationToken ct = default);
    Task UpdateAsync(Invoice invoice, CancellationToken ct = default);
    Task RemoveAsync(Invoice invoice, CancellationToken ct = default);

    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default);
}
