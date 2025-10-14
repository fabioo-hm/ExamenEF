using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IAuditoriaRepository
{
    Task<Auditoria?> GetByIdAsync(Guid Id, CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetByUserMemberIdAsync(int userMemberId, CancellationToken ct = default);
    Task<IReadOnlyList<Auditoria>> GetPagedAsync(int page,int size,string? q, CancellationToken ct = default);
    Task<int> CountAsync(string? q, CancellationToken ct = default);
    Task AddAsync(Auditoria auditoria, CancellationToken ct = default);
    Task UpdateAsync(Auditoria auditoria, CancellationToken ct = default);
    Task RemoveAsync(Auditoria auditoria, CancellationToken ct = default);
}