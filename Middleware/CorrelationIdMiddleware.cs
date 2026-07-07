using Microsoft.AspNetCore.Http;

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
            var cid = Guid.NewGuid().ToString();

            context.Response.Headers["X-Correlation-ID"] = cid;

            await _next(context);
        }
    }
}