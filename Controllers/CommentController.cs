using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace VulnerableApp.Controllers
{
    public class CommentController : Controller
    {
        private static List<string> _comments = new();
        private readonly ILogger<CommentController> _logger;

        public CommentController(ILogger<CommentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var stopwatch = Stopwatch.StartNew();
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("Inicio Comment.Index. Usuario:{User} IP:{IP}", userSession, ip);

            try
            {
                _logger.LogInformation("Fin Comment.Index - Retornando lista de comentarios. Cantidad:{Count}", _comments.Count);
                return View(_comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en Comment.Index al cargar los comentarios.");
                return StatusCode(500);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin Ejecución Comment.Index. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }

        [HttpPost]
        public IActionResult AddComment(string comment)
        {
            var stopwatch = Stopwatch.StartNew();
            var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("Inicio Comment.AddComment. Usuario:{User} IP:{IP} Parámetros:[commentLength={Length}]", userSession, ip, comment?.Length ?? 0);

            try
            {
                if (string.IsNullOrEmpty(comment))
                {
                    _logger.LogWarning("Comment.AddComment - Intento de guardar un comentario vacío o nulo por Usuario:{User}", userSession);
                }
                else
                {
                    // Ejemplo de validación básica para emitir un Warning de posible inyección o longitud
                    if (comment.Contains("<script>"))
                    {
                        _logger.LogWarning("Comment.AddComment - Alerta de posible XSS detectado en parámetro 'comment' por Usuario:{User}", userSession);
                    }

                    _comments.Add(comment);
                    _logger.LogInformation("Comment.AddComment - Comentario agregado con éxito.");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción en Comment.AddComment lanzado por el Usuario:{User}", userSession);
                return StatusCode(500);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Fin Ejecución Comment.AddComment. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}