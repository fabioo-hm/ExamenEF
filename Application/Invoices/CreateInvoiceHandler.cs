using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Invoices;

public sealed class CreateInvoiceHandler(IInvoiceRepository repo) : IRequestHandler<CreateInvoice, Guid>
{
    public async Task<Guid> Handle(CreateInvoice req, CancellationToken ct)
    {
        var invoice = new Invoice( req.ServiceOrderId, req.LaborCost, req.PartsTotal, req.PaymentMethod);
        await repo.AddAsync(invoice, ct);
        return invoice.Id;
    }
}    