using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using VulnerableApp.Data;
using VulnerableApp.Models;
using System.Diagnostics;
using System;

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
            var stopwatch = Stopwatch.StartNew();
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // Se registra Entrada, Usuario, IP, Ruta y Parámetros
            _logger.LogInformation("Inicio Search.Index. Entrada a la acción.");
            _logger.LogInformation("Usuario:{User} IP:{IP} Ruta:{Route} Parámetros:[search={SearchParam}]",
                userSession, ip, HttpContext.Request.Path, search);

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    _logger.LogWarning("Search.Index - Parámetro de búsqueda vacío enviado por Usuario:{User}", userSession);
                    _logger.LogInformation("Fin Search.Index - Salida: Retornando lista vacía.");
                    return View(new List<User>());
                }

                // Simulación para generar un Error/Excepción si buscan la palabra clave "error_test"
                if (search == "error_test")
                {
                    throw new InvalidOperationException("Simulación de error en la consulta a la base de datos de búsqueda.");
                }

                var users = _db.Users
                               .Where(u => u.Username.Contains(search))
                               .ToList();

                _logger.LogInformation("Fin Search.Index - Salida exitosa. Coincidencias encontradas: {Count}", users.Count);
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción capturada en Search.Index al buscar: {SearchParam}", search);
                return StatusCode(500, "Error interno durante la búsqueda");
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin Ejecución Search.Index. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}