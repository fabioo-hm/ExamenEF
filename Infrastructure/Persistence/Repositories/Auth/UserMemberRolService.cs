using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Auth;

public class UserMemberRolService : IUserMemberRolService
{
    private readonly AutoTallerDbContext _context;

    public UserMemberRolService(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserMemberRol>> GetAllAsync()
    {
        return await _context.UserMemberRols
            .Include(umr => umr.UserMembers)
            .Include(umr => umr.Rol)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<UserMemberRol?> GetByIdsAsync(int userMemberId, int roleId)
    {
        return await _context.UserMemberRols
            .Include(umr => umr.UserMembers)
            .Include(umr => umr.Rol)
            .FirstOrDefaultAsync(umr =>
                umr.UserMemberId == userMemberId &&
                umr.RolId == roleId);
    }

    public async Task AddAsync(UserMemberRol entity)
    {
        await _context.UserMemberRols.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<UserMemberRol> entities)
    {
        await _context.UserMemberRols.AddRangeAsync(entities);
    }

    public void Update(UserMemberRol entity)
    {
        _context.UserMemberRols.Update(entity);
    }

    public void Remove(UserMemberRol entity)
    {
        _context.UserMemberRols.Remove(entity);
    }

    public void RemoveRange(IEnumerable<UserMemberRol> entities)
    {
        _context.UserMemberRols.RemoveRange(entities);
    }

    public async Task<List<UserMemberRol>> FindAsync(Expression<Func<UserMemberRol, bool>> predicate)
    {
        return await _context.UserMemberRols
            .Include(umr => umr.Rol)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<List<UserMemberRol>> GetByUserIdAsync(int userMemberId)
    {
        return await _context.UserMemberRols
            .Include(umr => umr.Rol)
            .Where(umr => umr.UserMemberId == userMemberId)
            .ToListAsync();
    }
}