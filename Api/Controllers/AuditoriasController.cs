using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Auditorias;
using Application.Abstractions;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;
public class AuditoriasController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitofwork;
    private readonly IAuditoriaRepository _repository;


    public AuditoriasController(IMapper mapper, IUnitOfWork unitofwork, IAuditoriaRepository repository)
    {
        _mapper = mapper;
        _unitofwork = unitofwork;
        _repository = repository;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<AuditoriaDto>>> GetAll(CancellationToken ct)
    {
        var auditorias = await _unitofwork.Auditorias.GetAllAsync(ct);
        var dto = _mapper.Map<IEnumerable<AuditoriaDto>>(auditorias);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [DisableRateLimiting]
    public async Task<ActionResult<AuditoriaDto>> GetById(Guid id, CancellationToken ct)
    {
        var auditoria = await _unitofwork.Auditorias.GetByIdAsync(id, ct);
        if (auditoria is null) return NotFound();

        return Ok(_mapper.Map<AuditoriaDto>(auditoria));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuditoriaDto dto, CancellationToken ct = default)
    {
        var meeting = new Auditoria(dto.EntidadAfectada, dto.Accion, dto.Detalles, dto.FechaHora, dto.RegistroAfectadoId, dto.UserMemberId);
        await _repository.AddAsync(meeting, ct);

        var created = new AuditoriaDto(meeting.Id, dto.EntidadAfectada, dto.Accion, dto.FechaHora, dto.Detalles, dto.RegistroAfectadoId, dto.UserMemberId);
        return CreatedAtAction(nameof(GetById), new { id = meeting.Id }, created);
    }
}