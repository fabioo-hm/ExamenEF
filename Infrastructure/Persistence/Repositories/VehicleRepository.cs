using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class VehicleRepository : IVehicleRepository
{
    private readonly AutoTallerDbContext _context;

    public VehicleRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Vehicles
            .Include(v => v.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<Vehicle?> GetByVinAsync(string vin, CancellationToken ct = default)
    {
        return await _context.Vehicles
            .Include(v => v.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Vin == vin, ct);
    }

    public async Task<IReadOnlyList<Vehicle>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _context.Vehicles
            .Where(v => v.CustomerId == customerId)
            .Include(v => v.Customer)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Vehicles
            .Include(v => v.Customer)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Vehicle>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v =>
                v.Brand!.Contains(search) ||
                v.Model!.Contains(search) ||
                v.Vin!.Contains(search));
        }

        return await query
            .Include(v => v.Customer)
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(v =>
                v.Brand!.Contains(search) ||
                v.Model!.Contains(search) ||
                v.Vin!.Contains(search));
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsVinAsync(string vin, CancellationToken ct = default)
    {
        return await _context.Vehicles.AnyAsync(v => v.Vin == vin, ct);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        await _context.Vehicles.AddAsync(vehicle, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Vehicle vehicle, CancellationToken ct = default)
    {
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync(ct);
    }
}