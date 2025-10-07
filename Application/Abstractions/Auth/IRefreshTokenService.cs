using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Auth;

namespace Application.Abstractions.Auth;

public interface IRefreshTokenService
{
    Task<RefreshToken?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(int userId, CancellationToken ct = default);
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
    Task UpdateAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAsync(string token, CancellationToken ct = default);
    Task RemoveAsync(RefreshToken token, CancellationToken ct = default);
    Task RemoveExpiredAsync(CancellationToken ct = default);
}