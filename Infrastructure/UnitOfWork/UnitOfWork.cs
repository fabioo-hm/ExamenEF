using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Application.Abstractions.Auth;
using Infrastructure.Persistence;


namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AutoTallerDbContext _context;
    private ICustomerRepository? _customerRepository;
    private IUserMemberService? _userMemberService;
    private IOrderDetailRepository? _orderDetailRepository;
    private IServiceOrderRepository? _serviceOrderRepository;
    private ISparePartRepository? _sparePartRepository;
    private IVehicleRepository? _vehicleRepository;
    private IInvoiceRepository? _invoiceRepository;
    private IRolService? _rolService;
    private IRefreshTokenService? _refreshTokenService;
     private IUserMemberRolService? _userMemberRolService;
    public UnitOfWork(AutoTallerDbContext context)
    {
        _context = context;
    }
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
    // public IProductRepository Products
    // {
    //     get
    //     {
    //         if (_productRepository == null)
    //         {
    //             _productRepository = new ProductRepository(_context);
    //         }
    //         return _productRepository;
    //     }
    // }
    public IRefreshTokenService refreshTokenService => _refreshTokenService ??= new RefreshTokenService(_context);
    public IUserMemberService userMemberService => _userMemberService ??= new UserMemberService(_context);
    public IUserMemberRolService userMemberRolService => _userMemberRolService ??= new UserMemberRolService(_context);
    public IRolService rolservice => _rolService ??= new RolService(_context);
    public ICustomerRepository customerRepository => _customerRepository ??= new CustomerRepository(_context);
    public IInvoiceRepository invoiceRepository => _invoiceRepository ??= new InvoiceRepository(_context);
    public IOrderDetailRepository orderDetailRepositoryRepository => _orderDetailRepository ??= new OrderDetailRepository(_context);
    public IServiceOrderRepository serviceOrderRepository => _serviceOrderRepository ??= new ServieOrderRepository(_context);
    public ISparePartRepository sparePartRepository => _sparePartRepository ??= new SparePartRepository(_context);
    public IVehicleRepository vehicleRepository => _vehicleRepository ??= new VehicleRepository(_context);
    public IUserMemberRolService UserMemberRoles => throw new NotImplementedException();
}