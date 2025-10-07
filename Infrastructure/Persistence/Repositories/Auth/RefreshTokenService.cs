using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Auth;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Auth;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AutoTallerDbContext _context;

    public RefreshTokenService(AutoTallerDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .Include(r => r.UserMember)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .Include(r => r.UserMember)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Token == token, ct);
    }

    public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _context.RefreshTokens
            .Where(r => r.UserId == userId)
            .Include(r => r.UserMember)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
    {
        await _context.RefreshTokens.AddAsync(token, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RefreshToken token, CancellationToken ct = default)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(string token, CancellationToken ct = default)
    {
        var existingToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token, ct);

        if (existingToken != null)
        {
            existingToken.Revoked = DateTime.UtcNow;
            _context.RefreshTokens.Update(existingToken);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveAsync(RefreshToken token, CancellationToken ct = default)
    {
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveExpiredAsync(CancellationToken ct = default)
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(r => r.Expires < DateTime.UtcNow)
            .ToListAsync(ct);

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync(ct);
        }
    }
}