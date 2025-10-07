using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Vehicles;
public sealed class CreateVehiclesHandler(IVehicleRepository repo) : IRequestHandler<CreateVehicle, Guid>
{
    public async Task<Guid> Handle(CreateVehicle req, CancellationToken ct)
    {
        var vehicle = new Vehicle(req.Brand, req.Model, req.Year, req.Vin, req.Mileage, req.CustomerId);

        await repo.AddAsync(vehicle, ct);
        return vehicle.Id;
    }
}