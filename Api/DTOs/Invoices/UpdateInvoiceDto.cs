using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;

namespace Api.DTOs.Invoices;

public record UpdateInvoiceDto( Guid ServiceOrderId, decimal LaborCost, decimal PartsTotal, PaymentMethod PaymentMethod);