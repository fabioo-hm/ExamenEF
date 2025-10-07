using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.SpareParts;
public record CreateSparePartDto( string Code, string Description, int StockQuantity, decimal UnitPrice );
