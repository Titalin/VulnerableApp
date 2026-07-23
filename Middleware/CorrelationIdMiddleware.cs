using Microsoft.AspNetCore.Http;
using Serilog.Context; // <-- 1. IMPORTANTE: Agrega este namespace
using System;
using System.Threading.Tasks;

namespace VulnerableApp.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var cid))
            {
                cid = Guid.NewGuid().ToString();
            }

            context.Response.Headers["X-Correlation-ID"] = cid;

            using (LogContext.PushProperty("CorrelationId", cid.ToString()))
            {
                await _next(context);
            }
        }
    }
}