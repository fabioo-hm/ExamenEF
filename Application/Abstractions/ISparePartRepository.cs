using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface ISparePartRepository
{
    Task<SparePart?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SparePart?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<SparePart>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SparePart>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default);
    Task<int> CountAsync(string? search, CancellationToken ct = default);

    Task AddAsync(SparePart sparePart, CancellationToken ct = default);
    Task UpdateAsync(SparePart sparePart, CancellationToken ct = default);
    Task RemoveAsync(SparePart sparePart, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> IsInStockAsync(Guid sparePartId, int requiredQuantity, CancellationToken ct = default);
    Task UpdateStockAsync(Guid sparePartId, int quantityChange, CancellationToken ct = default);
}
