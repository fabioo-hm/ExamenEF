using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ServiceOrderRepository : IServiceOrderRepository
{
    private readonly AutoTallerDbContext _context;

    public ServiceOrderRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceOrder?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.Customer)
            .Include(o => o.UserMember)
            .Include(o => o.OrderDetails)
                .ThenInclude(d => d.SparePart)
            .Include(o => o.Invoice)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken ct = default)
    {
        return await _context.ServiceOrders
            .Where(o => o.VehicleId == vehicleId)
            .Include(o => o.Vehicle)
            .Include(o => o.UserMember)
            .Include(o => o.Invoice)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetByServiceTypeAsync(ServiceType serviceType, CancellationToken ct = default)
    {
        return await _context.ServiceOrders
            .Where(o => o.ServiceType == serviceType)
            .Include(o => o.Vehicle)
            .Include(o => o.UserMember)
            .Include(o => o.Invoice)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetByUserMemberIdAsync(int userMemberId, CancellationToken ct = default)
    {
        return await _context.ServiceOrders
            .Where(o => o.UserMemberId == userMemberId)
            .Include(o => o.Vehicle)
            .Include(o => o.Invoice)
            .Include(o => o.UserMember)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default)
    {
        return await _context.ServiceOrders
            .Where(o => o.OrderStatus == status)
            .Include(o => o.Vehicle)
            .Include(o => o.UserMember)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.Customer)
            .Include(o => o.UserMember)
            .Include(o => o.Invoice)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceOrder>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _context.ServiceOrders
            .Include(o => o.Vehicle)
                .ThenInclude(v => v.Customer)
            .Include(o => o.UserMember)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(o =>
                o.Vehicle!.Brand.Contains(search) ||
                o.Vehicle.Model.Contains(search));
        }

        return await query
            .OrderByDescending(o => o.EntryDate)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.ServiceOrders
            .Include(o => o.Vehicle)
            .Include(o => o.UserMember)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(o =>
                o.Vehicle!.Brand.Contains(search) ||
                o.Vehicle.Model.Contains(search));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(ServiceOrder order, CancellationToken ct)
    {
        try
        {
            await _context.ServiceOrders.AddAsync(order, ct);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error al guardar la orden: {ex.InnerException?.Message}");
            throw;
        }
    }

    public async Task UpdateAsync(ServiceOrder order, CancellationToken ct = default)
    {
        _context.ServiceOrders.Update(order);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(ServiceOrder order, CancellationToken ct = default)
    {
        _context.ServiceOrders.Remove(order);
        await _context.SaveChangesAsync(ct);
    }
}
