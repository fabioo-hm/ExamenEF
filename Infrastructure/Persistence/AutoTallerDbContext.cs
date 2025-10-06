using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AutoTallerDbContext(DbContextOptions<AutoTallerDbContext> options) : DbContext(options)
{
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UserMember> UserMembers => Set<UserMember>();
    public DbSet<UserMemberRol> UserMemberRols => Set<UserMemberRol>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<SparePart> SpareParts => Set<SparePart>();
    public DbSet<Customer> Customers => Set<Customer>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AutoTallerDbContext).Assembly);
}
