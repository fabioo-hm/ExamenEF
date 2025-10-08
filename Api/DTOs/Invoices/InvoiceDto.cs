using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;

namespace Api.DTOs.Invoices;

public record InvoiceDto( Guid Id, Guid ServiceOrderId, DateTime IssueDate, decimal LaborCost, decimal PartsTotal, decimal Total, PaymentMethod PaymentMethod);