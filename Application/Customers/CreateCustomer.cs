using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Customers;

public sealed record CreateCustomer(string Name, string Email, string Phone) : IRequest<Guid>;