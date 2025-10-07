using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.SpareParts;

public sealed record CreateSparePart(string Code,string Description, int StockQuantity, decimal UnitPrice ) : IRequest<Guid>;