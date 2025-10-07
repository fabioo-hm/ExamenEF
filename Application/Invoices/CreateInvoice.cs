using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Invoices;

public sealed record CreateInvoice(Guid ServiceOrderId, decimal LaborCost, decimal PartsTotal, PaymentMethod PaymentMethod) : IRequest<Guid>;