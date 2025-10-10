using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Auth;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class ServiceOrder
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }
    public OrderStatus OrderStatus { get; private set; }

    public ServiceType ServiceType { get; private set; }
    public int UserMemberId { get; private set; }
    public UserMember? UserMember { get; private set; }
    public DateTime EntryDate { get; private set; }
    public DateTime EstimatedDeliveryDate { get; private set; }

    public ICollection<OrderDetail> OrderDetails { get; private set; } = new List<OrderDetail>();
    public Invoice? Invoice { get; private set; }

    public ServiceOrder() { }

    public ServiceOrder(Guid vehicleId, ServiceType serviceType, int userMemberId, DateTime entryDate, DateTime estimatedDeliveryDate, OrderStatus orderStatus)
    {
        VehicleId = vehicleId;
        ServiceType = serviceType;
        OrderStatus = orderStatus;
        UserMemberId = userMemberId;
        EntryDate = entryDate;
        EstimatedDeliveryDate = estimatedDeliveryDate;
    }
}
