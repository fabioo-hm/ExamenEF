
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.SpareParts;
using Application.Abstractions;
using Application.SpareParts;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
public class SparePartsController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly ISparePartRepository _repo;

        public SparePartsController(IMediator mediator, ISparePartRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateSparePartDto dto, CancellationToken ct)
        {
            
            var exists = await _repo.ExistsByCodeAsync(dto.Code, ct);
            if (exists)
                return Conflict($"Ya existe una pieza con el código {dto.Code}.");

            var command = new CreateSparePart(dto.Code, dto.Description, dto.StockQuantity, dto.UnitPrice);
            var id = await _mediator.Send(command, ct);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SparePartDto>>> GetAll(CancellationToken ct)
        {
            var parts = await _repo.GetAllAsync(ct);
            var result = parts.Select(p => new SparePartDto(
                p.Id,
                p.Code ?? string.Empty,
                p.Description ?? string.Empty,
                p.StockQuantity,
                p.UnitPrice
            ));

            return Ok(result);
        }

        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SparePartDto>> GetById(Guid id, CancellationToken ct)
        {
            var part = await _repo.GetByIdAsync(id, ct);
            if (part is null)
                return NotFound();

            var dto = new SparePartDto(
                part.Id,
                part.Code ?? string.Empty,
                part.Description ?? string.Empty,
                part.StockQuantity,
                part.UnitPrice
            );

            return Ok(dto);
        }

        
        [HttpGet("code/{code}")]
        public async Task<ActionResult<SparePartDto>> GetByCode(string code, CancellationToken ct)
        {
            var part = await _repo.GetByCodeAsync(code, ct);
            if (part is null)
                return NotFound();

            var dto = new SparePartDto(
                part.Id,
                part.Code ?? string.Empty,
                part.Description ?? string.Empty,
                part.StockQuantity,
                part.UnitPrice
            );

            return Ok(dto);
        }

        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSparePartDto dto, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing is null)
                return NotFound();

            var updated = new SparePart(
                dto.Code ?? existing.Code ?? string.Empty,
                dto.Description ?? existing.Description ?? string.Empty,
                dto.StockQuantity,
                dto.UnitPrice
            );

            typeof(SparePart).GetProperty(nameof(SparePart.Id))!
                .SetValue(updated, id);

            await _repo.UpdateAsync(updated, ct);
            return NoContent();
        }

        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var part = await _repo.GetByIdAsync(id, ct);
            if (part is null)
                return NotFound();

            await _repo.RemoveAsync(part, ct);
            return NoContent();
        }

    
        [HttpPatch("{id:guid}/stock")]
        public async Task<IActionResult> UpdateStock(Guid id, [FromQuery] int quantityChange, CancellationToken ct)
        {
            var part = await _repo.GetByIdAsync(id, ct);
            if (part is null)
                return NotFound();

            await _repo.UpdateStockAsync(id, quantityChange, ct);
            return Ok($"Stock actualizado. Cambio: {quantityChange}");
        }
    }