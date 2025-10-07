using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Auth;

namespace Application.Abstractions;
public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }
    IOrderDetailRepository OrderDetails { get; }
    IInvoiceRepository Invoices { get; }
    IServiceOrderRepository ServiceOrders { get; }
    ISparePartRepository SpareParts { get; }
    IUserMemberService UserMembers { get; }
    IUserMemberRolService UserMemberRoles { get; }
    IRefreshTokenService RefreshTokens { get; }
    IRolService Roles { get; }
    IVehicleRepository Vehicles { get; }
    // Task<int> SaveAsync();
    Task<int> SaveChanges(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default);
}