using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;

public class SparePart
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? Code { get; private set; }
    public string? Description { get; private set; }
    public int StockQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public ICollection<OrderDetail> OrderDetails { get; private set; } = new List<OrderDetail>();

    public SparePart() { }

    public SparePart(string code, string description, int stockQuantity, decimal unitPrice)
    {
        Code = code;
        Description = description;
        StockQuantity = stockQuantity;
        UnitPrice = unitPrice;
    }
}