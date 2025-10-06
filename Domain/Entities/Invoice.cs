using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Invoice
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ServiceOrderId { get; private set; }
    public ServiceOrder? ServiceOrder { get; private set; }

    public DateTime IssueDate { get; private set; }
    public decimal LaborCost { get; private set; }
    public decimal PartsTotal { get; private set; }
    public decimal Total => LaborCost + PartsTotal;

    public Invoice() { }

    public Invoice(Guid serviceOrderId, decimal laborCost, decimal partsTotal)
    {
        ServiceOrderId = serviceOrderId;
        LaborCost = laborCost;
        PartsTotal = partsTotal;
        IssueDate = DateTime.Now;
    }
}
