using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Customers;

public sealed class CreateCustomerHandler(ICustomerRepository repo) : IRequestHandler<CreateCustomer, Guid>
{
    public async Task<Guid> Handle(CreateCustomer req, CancellationToken ct)
    {
        var customer = new Customer(req.Name, req.Email, req.Phone);
        await repo.AddAsync(customer, ct);
        return customer.Id;
    }
}