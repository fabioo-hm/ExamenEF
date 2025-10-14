

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.OrderDetails;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public class OrderDetailsController : BaseApiController
{
    private readonly IOrderDetailRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;

    public OrderDetailsController(IOrderDetailRepository repository,IMapper mapper, IUnitOfWork unitofwork)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetAll(CancellationToken ct)
    {
        var orderDetail = await _unitofwork.OrderDetails.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<OrderDetailDto>>(orderDetail);
        return Ok(dto);
    }

    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var orderDetail = await _repository.GetByIdAsync(id, ct);
        if (orderDetail is null)
            return NotFound(new { Message = "Order detail not found." });

        var dto = new OrderDetailDto(
            orderDetail.Id,
            orderDetail.ServiceOrderId,
            orderDetail.SparePartId,
            orderDetail.Quantity,
            orderDetail.UnitCost,
            orderDetail.Subtotal
        );

        return Ok(dto);
    }

    
    [HttpGet("by-serviceorder/{serviceOrderId:guid}")]
    public async Task<IActionResult> GetByServiceOrder(Guid serviceOrderId, CancellationToken ct = default)
    {
        var details = await _repository.GetByServiceOrderIdAsync(serviceOrderId, ct);

        var result = details.Select(od => new OrderDetailDto(
            od.Id,
            od.ServiceOrderId,
            od.SparePartId,
            od.Quantity,
            od.UnitCost,
            od.Subtotal
        ));

        return Ok(result);
    }

    
    [HttpGet("total/{serviceOrderId:guid}")]
    public async Task<IActionResult> GetTotal(Guid serviceOrderId, CancellationToken ct = default)
    {
        var total = await _repository.GetTotalCostByServiceOrderIdAsync(serviceOrderId, ct);
        return Ok(new { ServiceOrderId = serviceOrderId, Total = total });
    }

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDetailDto dto, CancellationToken ct = default)
    {
        var exists = await _repository.ExistsAsync(dto.ServiceOrderId, dto.SparePartId, ct);
        if (exists)
            return Conflict(new { Message = "This spare part is already added to the service order." });

        var orderDetail = new OrderDetail(dto.ServiceOrderId, dto.SparePartId, dto.Quantity, dto.UnitCost);
        await _repository.AddAsync(orderDetail, ct);

        var created = new OrderDetailDto(
            orderDetail.Id,
            orderDetail.ServiceOrderId,
            orderDetail.SparePartId,
            orderDetail.Quantity,
            orderDetail.UnitCost,
            orderDetail.Subtotal
        );

        return CreatedAtAction(nameof(GetById), new { id = orderDetail.Id }, created);
    }

    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderDetailDto dto, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(new { Message = "Order detail not found." });

        
        existing.GetType().GetProperty("ServiceOrderId")?.SetValue(existing, dto.ServiceOrderId);
        existing.GetType().GetProperty("SparePartId")?.SetValue(existing, dto.SparePartId);
        existing.GetType().GetProperty("Quantity")?.SetValue(existing, dto.Quantity);
        existing.GetType().GetProperty("UnitCost")?.SetValue(existing, dto.UnitCost);

        await _repository.UpdateAsync(existing, ct);

        var updated = new OrderDetailDto(
            existing.Id,
            existing.ServiceOrderId,
            existing.SparePartId,
            existing.Quantity,
            existing.UnitCost,
            existing.Subtotal
        );

        return Ok(updated);
    }

    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(new { Message = "Order detail not found." });

        await _repository.RemoveAsync(existing, ct);
        return NoContent();
    }
}
