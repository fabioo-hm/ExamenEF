
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Invoices;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class InvoicesController : BaseApiController
{
    private readonly IInvoiceRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;


    public InvoicesController(IInvoiceRepository repository,IMapper mapper, IUnitOfWork unitofwork)
    {
        _repository = repository;
        _unitofwork = unitofwork;
        _mapper = mapper;

    }

    // ✅ GET: api/invoices?page=1&size=10
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken ct = default)
    {
        var invoices = await _repository.GetPagedAsync(page, size, ct);
        var total = await _repository.CountAsync(ct);

        var result = invoices.Select(i => new InvoiceDto(
            i.Id,
            i.ServiceOrderId,
            i.IssueDate,
            i.LaborCost,
            i.PartsTotal,
            i.Total,
            i.PaymentMethod
        ));

        return Ok(new
        {
            Total = total,
            Page = page,
            Size = size,
            Data = result
        });
    }

    // ✅ GET: api/invoices/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var invoice = await _repository.GetByIdAsync(id, ct);
        if (invoice is null)
            return NotFound(new { Message = "Invoice not found." });

        var dto = new InvoiceDto(
            invoice.Id,
            invoice.ServiceOrderId,
            invoice.IssueDate,
            invoice.LaborCost,
            invoice.PartsTotal,
            invoice.Total,
            invoice.PaymentMethod
        );

        return Ok(dto);
    }
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll(CancellationToken ct)
    {
        var invoice = await _unitofwork.Invoices.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<InvoiceDto>>(invoice);
        return Ok(dto);
    }
    // ✅ POST: api/invoices
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto, CancellationToken ct = default)
    {
        var invoice = new Invoice(dto.ServiceOrderId, dto.LaborCost, dto.PartsTotal, dto.PaymentMethod);
        await _repository.AddAsync(invoice, ct);

        var created = new InvoiceDto(
            invoice.Id,
            invoice.ServiceOrderId,
            invoice.IssueDate,
            invoice.LaborCost,
            invoice.PartsTotal,
            invoice.Total,
            invoice.PaymentMethod
        );

        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, created);
    }

    // ✅ PUT: api/invoices/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceDto dto, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(new { Message = "Invoice not found." });

        // Actualizar valores
        existing.GetType().GetProperty("ServiceOrderId")?.SetValue(existing, dto.ServiceOrderId);
        existing.GetType().GetProperty("LaborCost")?.SetValue(existing, dto.LaborCost);
        existing.GetType().GetProperty("PartsTotal")?.SetValue(existing, dto.PartsTotal);
        existing.GetType().GetProperty("PaymentMethod")?.SetValue(existing, dto.PaymentMethod);

        await _repository.UpdateAsync(existing, ct);

        var updated = new InvoiceDto(
            existing.Id,
            existing.ServiceOrderId,
            existing.IssueDate,
            existing.LaborCost,
            existing.PartsTotal,
            existing.Total,
            existing.PaymentMethod
        );

        return Ok(updated);
    }

    // ✅ DELETE: api/invoices/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var existing = await _repository.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(new { Message = "Invoice not found." });

        await _repository.RemoveAsync(existing, ct);
        return NoContent();
    }
}

