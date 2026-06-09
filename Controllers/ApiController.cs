using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VulnerableApp.Data;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ApiController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            // SEGURO: Obtiene el ID del usuario en la sesión actual
            var currentUserId = HttpContext.Session.GetInt32("UserId");

            // SEGURO: Si no está logueado, deniega acceso inmediato
            if (!currentUserId.HasValue) return Unauthorized();

            // SEGURO: Control de acceso (IDOR) - Verifica si el usuario intenta ver un perfil ajeno
            if (id != currentUserId.Value) return Forbid();

            var user = _db.Users.Find(id);
            if (user == null) return NotFound();

            // SEGURO: Solo retorna campos no sensibles, omitiendo hashes o passwords
            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email
            });
        }
    }
}