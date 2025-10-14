
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.ServiceOrders;
using Application.Abstractions;
using Application.DTOs.ServiceOrders;
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
    public async Task<IActionResult> Create([FromBody] CreateServiceOrderDto dto, CancellationToken ct = default)
    {
        // Puedes agregar validaciones personalizadas si quieres:
        if (dto.VehicleId == Guid.Empty)
            return BadRequest(new { Message = "El ID del vehículo es obligatorio." });

        if (dto.UserMemberId <= 0)
            return BadRequest(new { Message = "El ID del mecánico es obligatorio." });

        // Crear entidad desde el DTO
        var serviceOrder = new ServiceOrder(
            dto.VehicleId,
            dto.ServiceType,
            dto.UserMemberId,
            dto.EntryDate,
            dto.EstimatedDeliveryDate,
            dto.OrderStatus
        );

        await _repo.AddAsync(serviceOrder, ct);

        // Opcional: devolver DTO creado
        var created = new ServiceOrderDto(
            serviceOrder.Id,
            serviceOrder.VehicleId,
            serviceOrder.ServiceType,
            serviceOrder.UserMemberId,
            serviceOrder.EntryDate,
            serviceOrder.EstimatedDeliveryDate,
            serviceOrder.OrderStatus
        );

        return CreatedAtAction(nameof(GetById), new { id = serviceOrder.Id }, created);
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
            o.UserMemberId,
            o.EstimatedDeliveryDate,
            o.EntryDate,
            o.OrderStatus
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
            order.UserMemberId,
            order.EntryDate,
            order.EstimatedDeliveryDate,
            order.OrderStatus
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

        // Actualizar la entidad existente en lugar de crear una nueva
        existing.Update(
            dto.VehicleId,
            dto.ServiceType,
            dto.UserMemberId,
            dto.EntryDate,
            dto.EstimatedDeliveryDate,
            dto.OrderStatus
        );

        await _repo.UpdateAsync(existing, ct);
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

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateServiceOrderStatusDto dto, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound();

        // Actualizar solo el estado
        existing.UpdateStatus(dto.OrderStatus);

        await _repo.UpdateAsync(existing, ct);
        return NoContent();
    }
}

