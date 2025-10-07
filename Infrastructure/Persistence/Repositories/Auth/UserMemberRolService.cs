using System;
using System.Collections.Generic;
using System.Linq;
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

    public void Update(UserMemberRol entity)
    {
        _context.UserMemberRols.Update(entity);
        _context.SaveChanges();
    }

    public void Remove(UserMemberRol entity)
    {
        _context.UserMemberRols.Remove(entity);
        _context.SaveChanges();
    }
}