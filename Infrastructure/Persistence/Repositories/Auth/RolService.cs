using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Auth;
public class RolService : IRolService
{
    private readonly AutoTallerDbContext _context;

    public RolService(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<Rol?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Roles
            .Include(r => r.UserMemberRols)
            .Include(r => r.UsersMembers)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<IEnumerable<Rol>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Roles
            .Include(r => r.UserMemberRols)
            .Include(r => r.UsersMembers)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public IEnumerable<Rol> Find(Expression<Func<Rol, bool>> expression)
    {
        return _context.Roles
            .Include(r => r.UserMemberRols)
            .Include(r => r.UsersMembers)
            .Where(expression)
            .AsEnumerable();
    }

    public async Task<IEnumerable<Rol>> GetPagedAsync(int pageIndex, int pageSize, string search, CancellationToken ct = default)
    {
        var query = _context.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => r.Name.Contains(search) || r.Description.Contains(search));
        }

        return await query
            .OrderBy(r => r.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? q, CancellationToken ct = default)
    {
        var query = _context.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(r => r.Name.Contains(q) || r.Description.Contains(q));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Rol entity, CancellationToken ct = default)
    {
        await _context.Roles.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Rol entity, CancellationToken ct = default)
    {
        _context.Roles.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Rol entity, CancellationToken ct = default)
    {
        _context.Roles.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }
}