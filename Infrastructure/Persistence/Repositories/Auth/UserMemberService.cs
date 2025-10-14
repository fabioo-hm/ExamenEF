using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Auth;
public class UserMemberService : IUserMemberService
{
    private readonly AutoTallerDbContext _context;

    public UserMemberService(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<UserMember?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.UserMembers
            .Include(u => u.Rols)
            .Include(u => u.UserMemberRols)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public IEnumerable<UserMember> Find(Expression<Func<UserMember, bool>> expression)
    {
        return _context.UserMembers
            .Include(u => u.Rols)
            .Include(u => u.UserMemberRols)
            .Include(u => u.RefreshTokens)
            .Where(expression)
            .AsEnumerable();
    }

    public async Task<IEnumerable<UserMember>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.UserMembers
            .Include(u => u.Rols)
            .Include(u => u.UserMemberRols)
            .Include(u => u.RefreshTokens)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<(int totalRegistros, IEnumerable<UserMember> registros)> GetPagedAsync(
        int pageIndex, int pageSize, string search)
    {
        var query = _context.UserMembers
            .Include(u => u.Rols)
            .Include(u => u.UserMemberRols)
            .Include(u => u.RefreshTokens)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                u.Username.Contains(search) ||
                u.Email.Contains(search));
        }

        var totalRegistros = await query.CountAsync();
        var registros = await query
            .OrderBy(u => u.Username)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        return (totalRegistros, registros);
    }

    public async Task<int> CountAsync(string? q, CancellationToken ct = default)
    {
        var query = _context.UserMembers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(u =>
                u.Username.Contains(q) ||
                u.Email.Contains(q));
        }

        return await query.CountAsync(ct);
    }

    public async Task AddAsync(UserMember entity, CancellationToken ct = default)
    {
        await _context.UserMembers.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UserMember entity, CancellationToken ct = default)
    {
        _context.UserMembers.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(UserMember entity, CancellationToken ct = default)
    {
        _context.UserMembers.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<UserMember?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return await _context.UserMembers
            .Include(u => u.UserMemberRols)
                .ThenInclude(ur => ur.Rol) // 👈 Esto es CLAVE
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Username == userName, ct);
    }

    public async Task<UserMember> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.UserMembers
            .Include(u => u.Rols)
            .Include(u => u.UserMemberRols)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u =>
                u.RefreshTokens.Any(rt => rt.Token == refreshToken));
    }
    public IQueryable<UserMember> GetAllWithRoles()
    {
        return _context.UserMembers
            .Include(u => u.UserMemberRols)
                .ThenInclude(ur => ur.Rol)
            .AsQueryable();
    }

}