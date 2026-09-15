using api.halper;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace api.middleware
{
    public class ExceptionsMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan Window = TimeSpan.FromSeconds(30);
        private const int Limit = 80;

        public ExceptionsMiddleware(RequestDelegate next, IHostEnvironment env, IMemoryCache cache)
        {
            _next = next;
            _env = env;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecurityHeaders(context);

                if (!IsRequestAllowed(context))
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";

                    var response = new ApiExceptions(429, "Too many requests. Please try again later.");
                    await context.Response.WriteAsJsonAsync(response);

                    return; // مهم جداً
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = _env.IsDevelopment()
                    ? new ApiExceptions(500, ex.Message, ex.StackTrace)
                    : new ApiExceptions(500, "Internal Server Error");

                await context.Response.WriteAsJsonAsync(response);
            }
        }

        private bool IsRequestAllowed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"RateLimit_{ip}";
            var now = DateTime.UtcNow;

            var entry = _cache.GetOrCreate(key, e =>
            {
                e.AbsoluteExpirationRelativeToNow = Window;
                return new RequestInfo
                {
                    Count = 0,
                    StartTime = now
                };
            });

            lock (entry)
            {
                if (now - entry.StartTime > Window)
                {
                    entry.StartTime = now;
                    entry.Count = 1;
                    return true;
                }

                if (entry.Count >= Limit)
                    return false;

                entry.Count++;
                return true;
            }
        }

        private void ApplySecurityHeaders(HttpContext context)
        {
            var headers = context.Response.Headers;

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
            headers["Content-Security-Policy"] = "default-src 'self'; img-src 'self' data: blob: https:; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://unpkg.com; style-src 'self' 'unsafe-inline' https://unpkg.com https://fonts.googleapis.com; font-src 'self' https://fonts.gstatic.com data:; connect-src 'self' http: https: ws:;";
        }

        private class RequestInfo
        {
            public int Count { get; set; }
            public DateTime StartTime { get; set; }
        }
    }
}
