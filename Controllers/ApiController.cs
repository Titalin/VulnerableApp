using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VulnerableApp.Data;
using System.Diagnostics;
using System;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApiController> _logger;

        public ApiController(AppDbContext db, ILogger<ApiController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("Inicio Api.GetUser. Usuario:{User} IP:{IP} Parámetros:[id={Id}]", userSession, ip, id);

            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserId");

                if (!currentUserId.HasValue)
                {
                    _logger.LogWarning("Api.GetUser - Intento de acceso no autenticado desde IP:{IP}", ip);
                    return Unauthorized();
                }

                if (id != currentUserId.Value)
                {
                    _logger.LogWarning("Api.GetUser - Posible ataque IDOR detectado. Usuario:{User} (ID:{CurrentUserId}) intentó acceder al ID:{RequestedId}", userSession, currentUserId.Value, id);
                    return Forbid();
                }

                var user = _db.Users.Find(id);
                if (user == null)
                {
                    _logger.LogWarning("Api.GetUser - Usuario no encontrado con ID:{Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Fin Api.GetUser - Exitoso. Salida: HTTP 200 OK");
                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción crítica en Api.GetUser para ID:{Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin Ejecución Api.GetUser. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}