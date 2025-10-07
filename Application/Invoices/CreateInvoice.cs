using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Invoices;

public sealed record CreateCompany(DateTime IssueDate,decimal LaborCost,decimal PartsTotal, decimal Total, Guid ServiceOrderId) : IRequest<Guid>;