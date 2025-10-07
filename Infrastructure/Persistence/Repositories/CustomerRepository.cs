using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AutoTallerDbContext _context;

    public CustomerRepository(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Email!.ToLower() == email.ToLower(), ct);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Customers
            .Include(c => c.Vehicles)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Customer>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.Name!.Contains(search) ||
                c.Email!.Contains(search) ||
                c.Phone!.Contains(search));
        }

        return await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .Include(c => c.Vehicles)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? search, CancellationToken ct = default)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                c.Name!.Contains(search) ||
                c.Email!.Contains(search) ||
                c.Phone!.Contains(search));
        }

        return await query.CountAsync(ct);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _context.Customers
            .AnyAsync(c => c.Email!.ToLower() == email.ToLower(), ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await _context.Customers.AddAsync(customer, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Customer customer, CancellationToken ct = default)
    {
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(ct);
    }
}