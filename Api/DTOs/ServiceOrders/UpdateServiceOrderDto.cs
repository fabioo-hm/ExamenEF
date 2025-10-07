using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.ServiceOrders;
public record UpdateServiceOrderDto( Guid? VehicleId, int? ServiceType, string? MechanicAssigned, DateTime? EntryDate, DateTime? EstimatedDeliveryDate);