using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities;
public class OrderDetail
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid ServiceOrderId { get; private set; }
    public ServiceOrder? ServiceOrder { get; private set; }

    public Guid SparePartId { get; private set; }
    public SparePart? SparePart { get; private set; }

    public int Quantity { get; private set; }
    public decimal UnitCost { get; private set; }

    public decimal Subtotal => Quantity * UnitCost;

    public OrderDetail() { }

    public OrderDetail(Guid serviceOrderId, Guid sparePartId, int quantity, decimal unitCost)
    {
        ServiceOrderId = serviceOrderId;
        SparePartId = sparePartId;
        Quantity = quantity;
        UnitCost = unitCost;
    }
}
