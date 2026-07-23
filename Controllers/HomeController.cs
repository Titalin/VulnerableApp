using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp.Models;
using System;

namespace VulnerableApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var stopwatch = Stopwatch.StartNew();
        var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        _logger.LogInformation("Inicio Home.Index. Usuario:{User} IP:{IP}", userSession, ip);

        try
        {
            _logger.LogInformation("Fin Home.Index - Renderizando vista principal.");
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción en Home.Index");
            return StatusCode(500);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Fin Ejecución Home.Index. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }
    }

    public IActionResult Privacy()
    {
        var stopwatch = Stopwatch.StartNew();
        var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        _logger.LogInformation("Inicio Home.Privacy. Usuario:{User} IP:{IP}", userSession, ip);

        try
        {
            _logger.LogInformation("Fin Home.Privacy - Renderizando política de privacidad.");
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción en Home.Privacy");
            return StatusCode(500);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Fin Ejecución Home.Privacy. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var stopwatch = Stopwatch.StartNew();
        var userSession = HttpContext.Session.GetString("User") ?? "Anónimo";
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        _logger.LogInformation("Inicio Home.Error - Manejo global de excepciones.");

        try
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            // Forzamos un registro de tipo Error en la salida ya que el flujo cayó aquí
            _logger.LogError("Se ha gatillado la ruta de errores. RequestId de diagnóstico: {RequestId} solicitado por Usuario:{User}", requestId, userSession);

            return View(new ErrorViewModel { RequestId = requestId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo catastrófico en el propio manejador de errores.");
            return StatusCode(500);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Fin Ejecución Home.Error. Tiempo de ejecución:{ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}