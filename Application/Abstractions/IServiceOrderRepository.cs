using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Abstractions;
public interface IServiceOrderRepository
{
    Task<ServiceOrder?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceOrder>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceOrder>> GetByServiceTypeAsync(ServiceType serviceType, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceOrder>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ServiceOrder>> GetPagedAsync(int page, int size, string? search, CancellationToken ct = default);
    Task<int> CountAsync(string? search, CancellationToken ct = default);
    Task AddAsync(ServiceOrder order, CancellationToken ct = default);
    Task UpdateAsync(ServiceOrder order, CancellationToken ct = default);
    Task RemoveAsync(ServiceOrder order, CancellationToken ct = default);
}
