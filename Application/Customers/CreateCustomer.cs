using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.customers;

public sealed record CreateCompany(string Name,string Email,string Phone) : IRequest<Guid>;