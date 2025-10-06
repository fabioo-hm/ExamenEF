using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class ServiceOrder
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    public ServiceType ServiceType { get; private set; }
    public string? MechanicAssigned { get; private set; }
    public DateTime EntryDate { get; private set; }
    public DateTime EstimatedDeliveryDate { get; private set; }

    public ICollection<OrderDetail> OrderDetails { get; private set; } = new List<OrderDetail>();
    public Invoice? Invoice { get; private set; }

    public ServiceOrder() { }

    public ServiceOrder(Guid vehicleId, ServiceType serviceType, string mechanicAssigned, DateTime entryDate, DateTime estimatedDeliveryDate)
    {
        VehicleId = vehicleId;
        ServiceType = serviceType;
        MechanicAssigned = mechanicAssigned;
        EntryDate = entryDate;
        EstimatedDeliveryDate = estimatedDeliveryDate;
    }
}
