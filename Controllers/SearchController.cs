using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using VulnerableApp.Data;
using VulnerableApp.Models;

namespace VulnerableApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchController> _logger;

        public SearchController(AppDbContext db, ILogger<SearchController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(string search)
        {
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation(
                "Usuario:{User} IP:{IP} Ruta:{Route} Parámetros:[search={Search}]",
                userSession,
                ip,
                HttpContext.Request.Path,
                search);

            if (string.IsNullOrWhiteSpace(search))
            {
                _logger.LogWarning(
                    "Parámetro de búsqueda vacío enviado por {User}",
                    userSession);

                return View(new List<User>());
            }

            // Para probar el ExceptionMiddleware
            if (search == "error_test")
            {
                throw new InvalidOperationException(
                    "Simulación de error en la búsqueda.");
            }

            var users = _db.Users
                           .Where(u => u.Username.Contains(search))
                           .ToList();

            _logger.LogInformation(
                "Búsqueda finalizada. Coincidencias: {Count}",
                users.Count);

            return View(users);
        }
    }
}