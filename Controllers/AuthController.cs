using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using VulnerableApp.Data;
using System.Diagnostics;
using System;

namespace VulnerableApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext db, ILogger<AuthController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Login()
        {
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("Inicio Auth.Login [GET]. Usuario:{User} IP:{IP}", userSession, ip);
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // SEGURIDAD: Se registra el parámetro username, pero se EXCLUYE estrictamente la contraseña del log.
            _logger.LogInformation("Inicio Auth.Login [POST]. Evento Autenticación - Intento de acceso para Usuario:{User} IP:{IP}", username, ip);

            try
            {
                var user = _db.Users.FirstOrDefault(u => u.Username == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("Evento Autenticación - FALLIDO. Credenciales inválidas para el Usuario:{User} desde IP:{IP}", username, ip);
                    ViewBag.Error = "Credenciales inválidas";
                    return View();
                }

                HttpContext.Session.SetString("User", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);

                _logger.LogInformation("Evento Autenticación - ÉXITO. Usuario:{User} ha iniciado sesión correctamente desde IP:{IP}", username, ip);
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción crítica durante el proceso de Autenticación para el Usuario:{User}", username);
                return StatusCode(500);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin Ejecución Auth.Login [POST]. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }

        public IActionResult Dashboard()
        {
            var stopwatch = Stopwatch.StartNew();
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("Inicio Auth.Dashboard. Usuario:{User} IP:{IP}", userSession, ip);

            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Auth.Dashboard - Intento de acceso denegado a Dashboard. Redirigiendo a Login. IP:{IP}", ip);
                    return RedirectToAction("Login");
                }

                var user = _db.Users.Find(userId.Value);
                _logger.LogInformation("Fin Auth.Dashboard - Carga exitosa del modelo para Usuario:{User}", userSession);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en Auth.Dashboard para el Usuario:{User}", userSession);
                return StatusCode(500);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin Ejecución Auth.Dashboard. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }

        public IActionResult Logout()
        {
            var stopwatch = Stopwatch.StartNew();
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("Inicio Auth.Logout. Evento Autenticación - Cierre de sesión solicitado por Usuario:{User} IP:{IP}", userSession, ip);

            try
            {
                HttpContext.Session.Clear();
                _logger.LogInformation("Evento Autenticación - Cierre de sesión Exitoso. Sesión limpiada.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en Auth.Logout provocado por Usuario:{User}", userSession);
                return StatusCode(500);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin Ejecución Auth.Logout. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}