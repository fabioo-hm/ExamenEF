using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class OrderDetailRepository : IOrderDetailRepository
{
    private readonly AutoTallerDbContext _context;

    public OrderDetailRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDetail?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .Include(od => od.ServiceOrder)
            .Include(od => od.SparePart)
            .FirstOrDefaultAsync(od => od.Id == id, ct);
    }

    public async Task<IReadOnlyList<OrderDetail>> GetByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .Include(od => od.SparePart)
            .Where(od => od.ServiceOrderId == serviceOrderId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<OrderDetail>> GetBySparePartIdAsync(Guid sparePartId, CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .Include(od => od.ServiceOrder)
            .Where(od => od.SparePartId == sparePartId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<OrderDetail>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .Include(od => od.ServiceOrder)
            .Include(od => od.SparePart)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(OrderDetail orderDetail, CancellationToken ct = default)
    {
        await _context.OrderDetails.AddAsync(orderDetail, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(OrderDetail orderDetail, CancellationToken ct = default)
    {
        _context.OrderDetails.Update(orderDetail);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(OrderDetail orderDetail, CancellationToken ct = default)
    {
        _context.OrderDetails.Remove(orderDetail);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetTotalCostByServiceOrderIdAsync(Guid serviceOrderId, CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .Where(od => od.ServiceOrderId == serviceOrderId)
            .SumAsync(od => od.Subtotal, ct);
    }

    public async Task<bool> ExistsAsync(Guid serviceOrderId, Guid sparePartId, CancellationToken ct = default)
    {
        return await _context.OrderDetails
            .AnyAsync(od => od.ServiceOrderId == serviceOrderId && od.SparePartId == sparePartId, ct);
    }
}