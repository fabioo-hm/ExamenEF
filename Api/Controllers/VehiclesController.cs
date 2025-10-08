
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Vehicles;
using Application.Abstractions;
using Application.Vehicles;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class VehiclesController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IVehicleRepository _vehicleRepository;

    public VehiclesController(IMediator mediator, IVehicleRepository vehicleRepository)
    {
        _mediator = mediator;
        _vehicleRepository = vehicleRepository;
    }

    // ============================================================
    // GET: api/vehicles
    // ============================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll(CancellationToken ct)
    {
        var vehicles = await _vehicleRepository.GetAllAsync(ct);

        var result = vehicles.Select(v => new VehicleDto(
            v.Id,
            v.Brand!,
            v.Model!,
            v.Year,
            v.Vin!,
            v.Mileage,
            v.CustomerId,
            v.Customer?.Name
        ));

        return Ok(result);
    }

    // ============================================================
    // GET: api/vehicles/{id}
    // ============================================================
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VehicleDto>> GetById(Guid id, CancellationToken ct)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id, ct);

        if (vehicle is null)
            return NotFound();

        var result = new VehicleDto(
            vehicle.Id,
            vehicle.Brand!,
            vehicle.Model!,
            vehicle.Year,
            vehicle.Vin!,
            vehicle.Mileage,
            vehicle.CustomerId,
            vehicle.Customer?.Name
        );

        return Ok(result);
    }

    // ============================================================
    // POST: api/vehicles
    // ============================================================
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateVehicleDto dto, CancellationToken ct)
    {
        var command = new CreateVehicle(dto.Brand, dto.Model, dto.Year, dto.Vin, dto.Mileage, dto.CustomerId);
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    // ============================================================
    // PUT: api/vehicles/{id}
    // ============================================================
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleDto dto, CancellationToken ct)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id, ct);
        if (vehicle is null)
            return NotFound();

        // Actualizamos los campos
        typeof(Vehicle).GetProperty("Brand")?.SetValue(vehicle, dto.Brand ?? vehicle.Brand);
        typeof(Vehicle).GetProperty("Model")?.SetValue(vehicle, dto.Model ?? vehicle.Model);
        typeof(Vehicle).GetProperty("Vin")?.SetValue(vehicle, dto.Vin ?? vehicle.Vin);
        typeof(Vehicle).GetProperty("Year")?.SetValue(vehicle, dto.Year != 0 ? dto.Year : vehicle.Year);
        typeof(Vehicle).GetProperty("Mileage")?.SetValue(vehicle, dto.Mileage != 0 ? dto.Mileage : vehicle.Mileage);

        await _vehicleRepository.UpdateAsync(vehicle, ct);
        return NoContent();
    }

    // ============================================================
    // DELETE: api/vehicles/{id}
    // ============================================================
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id, ct);
        if (vehicle is null)
            return NotFound();

        await _vehicleRepository.RemoveAsync(vehicle, ct);
        return NoContent();
    }
}
