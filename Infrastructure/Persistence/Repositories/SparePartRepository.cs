using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class SparePartRepository : ISparePartRepository
{
    private readonly AutoTallerDbContext _context;

    public SparePartRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<SparePart?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.SpareParts
            .Include(p => p.OrderDetails)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<SparePart?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.SpareParts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Code == code, ct);
    }

    public async Task<IReadOnlyList<SparePart>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.SpareParts
            .AsNoTracking()
            .OrderBy(p => p.Code)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<SparePart>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _context.SpareParts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Code!.Contains(search) ||
                p.Description!.Contains(search));
        }

        return await query
            .OrderBy(p => p.Code)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.SpareParts.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Code!.Contains(search) ||
                p.Description!.Contains(search));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(SparePart sparePart, CancellationToken ct = default)
    {
        await _context.SpareParts.AddAsync(sparePart, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(SparePart sparePart, CancellationToken ct = default)
    {
        _context.SpareParts.Update(sparePart);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(SparePart sparePart, CancellationToken ct = default)
    {
        _context.SpareParts.Remove(sparePart);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.SpareParts
            .AnyAsync(p => p.Code == code, ct);
    }

    public async Task<bool> IsInStockAsync(Guid sparePartId, int requiredQuantity, CancellationToken ct = default)
    {
        var part = await _context.SpareParts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == sparePartId, ct);

        return part != null && part.StockQuantity >= requiredQuantity;
    }

    public async Task UpdateStockAsync(Guid sparePartId, int quantityChange, CancellationToken ct = default)
    {
        var part = await _context.SpareParts.FirstOrDefaultAsync(p => p.Id == sparePartId, ct);
        if (part == null) return;

        part.GetType()
            .GetProperty("StockQuantity")!
            .SetValue(part, part.StockQuantity + quantityChange);

        _context.SpareParts.Update(part);
        await _context.SaveChangesAsync(ct);
    }
}