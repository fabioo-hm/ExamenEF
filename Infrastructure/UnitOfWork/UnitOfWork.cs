using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Abstractions.Auth;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Auth;

namespace Infrastructure.UnitOfWork;
public class UnitOfWork : IUnitOfWork
{
    private readonly AutoTallerDbContext _context;

    private ICustomerRepository? _customers;
    private IOrderDetailRepository? _orderDetails;
    private IInvoiceRepository? _invoices;
    private IServiceOrderRepository? _serviceOrders;
    private ISparePartRepository? _spareParts;
    private IVehicleRepository? _vehicles;
    private IUserMemberService? _userMembers;
    private IUserMemberRolService? _userMemberRoles;
    private IRefreshTokenService? _refreshTokens;
    private IRolService? _roles;

    public UnitOfWork(AutoTallerDbContext context)
    {
        _context = context;
    }

    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
    public IOrderDetailRepository OrderDetails => _orderDetails ??= new OrderDetailRepository(_context);
    public IInvoiceRepository Invoices => _invoices ??= new InvoiceRepository(_context);
    public IServiceOrderRepository ServiceOrders => _serviceOrders ??= new ServiceOrderRepository(_context);
    public ISparePartRepository SpareParts => _spareParts ??= new SparePartRepository(_context);
    public IVehicleRepository Vehicles => _vehicles ??= new VehicleRepository(_context);


    public IUserMemberService UserMembers => _userMembers ??= new UserMemberService(_context);
    public IUserMemberRolService UserMemberRoles => _userMemberRoles ??= new UserMemberRolService(_context);
    public IRefreshTokenService RefreshTokens => _refreshTokens ??= new RefreshTokenService(_context);
    public IRolService Roles => _roles ??= new RolService(_context);

    public Task<int> SaveChanges(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken ct = default)
    {
        await using var tx = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            await operation(ct);
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}