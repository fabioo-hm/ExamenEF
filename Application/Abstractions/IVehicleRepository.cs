using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Abstractions;
public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Vehicle?> GetByVinAsync(string vin, CancellationToken ct = default);
    Task<IReadOnlyList<Vehicle>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Vehicle>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default);
    Task<int> CountAsync(string? search, CancellationToken ct = default);
    Task<bool> ExistsVinAsync(string vin, CancellationToken ct = default);
    Task AddAsync(Vehicle vehicle, CancellationToken ct = default);
    Task UpdateAsync(Vehicle vehicle, CancellationToken ct = default);
    Task RemoveAsync(Vehicle vehicle, CancellationToken ct = default);
}
