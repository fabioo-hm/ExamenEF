using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.OrderDetails;

public record UpdateOrderDetailDto( Guid ServiceOrderId, Guid SparePartId, int Quantity, decimal UnitCost);