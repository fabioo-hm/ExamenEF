using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class InvoiceRepository : IInvoiceRepository
{
    private readonly AutoTallerDbContext _context;

    public InvoiceRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Include(i => i.ServiceOrder)
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task<Invoice?> GetByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Include(i => i.ServiceOrder)
            .FirstOrDefaultAsync(i => i.ServiceOrderId == serviceOrderId, ct);
    }

    public async Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Invoices
            .Include(i => i.ServiceOrder)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Include(i => i.ServiceOrder)
            .Where(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Invoice>> GetByPaymentMethodAsync(PaymentMethod method, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Include(i => i.ServiceOrder)
            .Where(i => i.PaymentMethod == method)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
    {
        await _context.Invoices.AddAsync(invoice, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Invoice invoice, CancellationToken ct = default)
    {
        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        return await _context.Invoices
            .Where(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
            .SumAsync(i => i.Total, ct);
    }
}