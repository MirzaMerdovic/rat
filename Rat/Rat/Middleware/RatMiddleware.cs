using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rat.Clients;
using System;
using System.Threading.Tasks;

namespace Rat.Middleware
{
    public class RatMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRatClient _rat;
        private readonly ILogger _logger;

        public RatMiddleware(RequestDelegate next, IRatClient rat, ILogger<RatMiddleware> logger)
        {
            _next = next;
            _rat = rat;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Equals("/rat/notify", StringComparison.InvariantCultureIgnoreCase))
            {
                _rat.ClearCache();
            }

            // TODO: Return OK response.
            await _next.Invoke(context).ConfigureAwait(false);
        }
    }
}
