using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly AutoTallerDbContext _context;

        public AuditoriaRepository(AutoTallerDbContext context)
        {
            _context = context;
        }

        public async Task<Auditoria?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Auditorias
                .Include(a => a.UserMember)
                .FirstOrDefaultAsync(a => a.Id == id, ct);
        }

        public async Task<IReadOnlyList<Auditoria>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Auditorias
                .Include(a => a.UserMember)
                .OrderByDescending(a => a.FechaHora)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Auditoria>> GetByUserMemberIdAsync(int userMemberId, CancellationToken ct = default)
        {
            return await _context.Auditorias
                .Where(a => a.UserMemberId == userMemberId)
                .OrderByDescending(a => a.FechaHora)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Auditoria>> GetPagedAsync(int page, int size, string? q, CancellationToken ct = default)
        {
            var query = _context.Auditorias
                .Include(a => a.UserMember)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a =>
                    a.EntidadAfectada.Contains(q) ||
                    a.Accion.Contains(q) ||
                    a.Detalles.Contains(q));
            }

            return await query
                .OrderByDescending(a => a.FechaHora)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);
        }

        public async Task<int> CountAsync(string? q, CancellationToken ct = default)
        {
            var query = _context.Auditorias.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a =>
                    a.EntidadAfectada.Contains(q) ||
                    a.Accion.Contains(q) ||
                    a.Detalles.Contains(q));
            }

            return await query.CountAsync(ct);
        }

        public async Task AddAsync(Auditoria auditoria, CancellationToken ct = default)
        {
            await _context.Auditorias.AddAsync(auditoria, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Auditoria auditoria, CancellationToken ct = default)
        {
            _context.Auditorias.Update(auditoria);
            await _context.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(Auditoria auditoria, CancellationToken ct = default)
        {
            _context.Auditorias.Remove(auditoria);
            await _context.SaveChangesAsync(ct);
        }
    }
}