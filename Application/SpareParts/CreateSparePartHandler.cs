using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.SpareParts;
public sealed class CreateSparePartHandler(ISparePartRepository repo) : IRequestHandler<CreateSparePart, Guid>
{
    public async Task<Guid> Handle(CreateSparePart req, CancellationToken ct)
    {
        var sparePart = new SparePart(req.Code, req.Description, req.StockQuantity, req.UnitPrice);
        await repo.AddAsync(sparePart, ct);
        return sparePart.Id;
    }
}