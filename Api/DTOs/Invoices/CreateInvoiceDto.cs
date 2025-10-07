using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.Invoices;
public record CreateInvoiceDto( Guid ServiceOrderId, decimal LaborCost, decimal PartsTotal, int PaymentMethod);