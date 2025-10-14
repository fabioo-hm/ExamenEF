using System.Text.Json;
using Application.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Api.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            var request = context.Request;
            var path = request.Path.Value ?? string.Empty;
            var method = request.Method;

            await _next(context);

            try
            {
                if (method is "POST" or "PUT" or "DELETE")
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    int userMemberId = userIdClaim != null ? int.Parse(userIdClaim) : 0;

                    string entidad = path.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Desconocida";

                    string detalles = $"{method} en {path}";
                    DateTime fecha = DateTime.UtcNow;

                    var auditoria = new Auditoria(
                        entidadAfectada: entidad,
                        accion: method,
                        detalles: detalles,
                        fechaHora: fecha,
                        userMemberId: userMemberId,
                        registroAfectadoId: 0
                    );

                    await unitOfWork.Auditorias.AddAsync(auditoria, default);
                    await unitOfWork.SaveChanges(default);

                    _logger.LogInformation("🧾 Auditoría registrada: {Entidad} - {Acción}", entidad, method);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registrando auditoría");
            }
        }
    }
}
