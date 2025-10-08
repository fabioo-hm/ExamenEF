
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Customers;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class CustomersController : BaseApiController
{
    private readonly ICustomerRepository _repository;

    public CustomersController(ICustomerRepository repository)
    {
        _repository = repository;
    }

    // ✅ GET: api/customers?page=1&size=10&search=John
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var customers = await _repository.GetPagedAsync(page, size, search, ct);
        var total = await _repository.CountAsync(search, ct);

        var result = customers.Select(c => new CustomerDto(
            c.Id,
            c.Name ?? string.Empty,
            c.Email ?? string.Empty,
            c.Phone ?? string.Empty
        ));

        return Ok(new
        {
            Total = total,
            Page = page,
            Size = size,
            Data = result
        });
    }

    // ✅ GET: api/customers/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var customer = await _repository.GetByIdAsync(id, ct);
        if (customer is null)
            return NotFound(new { Message = "Customer not found." });

        var dto = new CustomerDto(customer.Id, customer.Name!, customer.Email!, customer.Phone ?? "");
        return Ok(dto);
    }

    // ✅ POST: api/customers
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto, CancellationToken ct = default)
    {
        if (await _repository.ExistsByEmailAsync(dto.Email, ct))
            return Conflict(new { Message = "A customer with that email already exists." });

        var customer = new Customer(dto.Name, dto.Email, dto.Phone);
        await _repository.AddAsync(customer, ct);

        var created = new CustomerDto(customer.Id, customer.Name!, customer.Email!, customer.Phone ?? "");
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, created);
    }

    // ✅ PUT: api/customers/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto dto, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(new { Message = "Customer not found." });

        // Actualización por reflexión para propiedades privadas (si tus setters son privados)
        if (dto.Name is not null)
            existing.GetType().GetProperty("Name")?.SetValue(existing, dto.Name);
        if (dto.Email is not null)
            existing.GetType().GetProperty("Email")?.SetValue(existing, dto.Email);
        if (dto.Phone is not null)
            existing.GetType().GetProperty("Phone")?.SetValue(existing, dto.Phone);

        await _repository.UpdateAsync(existing, ct);

        var updated = new CustomerDto(existing.Id, existing.Name!, existing.Email!, existing.Phone ?? "");
        return Ok(updated);
    }

    // ✅ DELETE: api/customers/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(new { Message = "Customer not found." });

        await _repository.RemoveAsync(existing, ct);
        return NoContent();
    }
}