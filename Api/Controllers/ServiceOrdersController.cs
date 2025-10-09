
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.ServiceOrders;
using Application.Abstractions;
using Application.ServiceOrders;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public class ServiceOrdersController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly IServiceOrderRepository _repo;

    public ServiceOrdersController(IMediator mediator, IServiceOrderRepository repo)
    {
        _mediator = mediator;
        _repo = repo;
    }

    // ✅ POST: api/serviceorders
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateServiceOrderDto dto, CancellationToken ct)
    {
        var command = new CreateServiceOrder(
            dto.VehicleId,
            dto.ServiceType,
            dto.MechanicAssigned,
            dto.EntryDate,
            dto.EstimatedDeliveryDate
        );

        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    // ✅ GET: api/serviceorders/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ServiceOrderDto>>> GetAll(CancellationToken ct)
    {
        var orders = await _repo.GetAllAsync(ct);
        var result = orders.Select(o => new ServiceOrderDto(
            o.Id,
            o.VehicleId,
            o.ServiceType,
            o.MechanicAssigned ?? string.Empty,
            o.EntryDate,
            o.EstimatedDeliveryDate
        ));

        return Ok(result);
    }

    // ✅ GET: api/serviceorders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServiceOrderDto>> GetById(Guid id, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(id, ct);
        if (order is null)
            return NotFound();

        var dto = new ServiceOrderDto(
            order.Id,
            order.VehicleId,
            order.ServiceType,
            order.MechanicAssigned ?? string.Empty,
            order.EntryDate,
            order.EstimatedDeliveryDate
        );

        return Ok(dto);
    }

    // ✅ PUT: api/serviceorders/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateServiceOrderDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound();

        // Actualizamos campos usando métodos o constructor nuevo
        var updated = new ServiceOrder(
            dto.VehicleId,
            dto.ServiceType,
            dto.MechanicAssigned ?? existing.MechanicAssigned ?? string.Empty,
            dto.EntryDate,
            dto.EstimatedDeliveryDate
        );

        // Reasignamos el ID existente
        typeof(ServiceOrder)
            .GetProperty(nameof(ServiceOrder.Id))!
            .SetValue(updated, id);

        await _repo.UpdateAsync(updated, ct);
        return NoContent();
    }

    // ✅ DELETE: api/serviceorders/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound();

        await _repo.RemoveAsync(existing, ct);
        return NoContent();
    }
}

