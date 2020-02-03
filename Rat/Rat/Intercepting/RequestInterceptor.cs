using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Intercepting
{
    public sealed class RequestsInterceptor : DelegatingHandler
    {
        private readonly char[] _splitter = new[] { '/' };

        private readonly Collection<RequestInterceptorOptions> _options;

        public RequestsInterceptor(IOptions<Collection<RequestInterceptorOptions>> options)
        {
            _options = options?.Value ?? new Collection<RequestInterceptorOptions>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            foreach (var option in _options.Where(x => x.MethodName.Equals(request.Method.Method, StringComparison.InvariantCultureIgnoreCase)))
            {
                var path = option.Path.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) ? option.Path : $"/{option.Path}";

                if (path.Contains("*", StringComparison.InvariantCultureIgnoreCase))
                {
                    var matchSegments = path.Split(_splitter, StringSplitOptions.RemoveEmptyEntries);
                    var segments = request.RequestUri.PathAndQuery.Split(_splitter, StringSplitOptions.RemoveEmptyEntries);

                    var isMatch = matchSegments.Length == segments.Length;

                    if (!isMatch)
                        continue;

                    for (var i = 0; i < matchSegments.Length; i++)
                    {
                        var match = matchSegments[i];
                        var segment = segments[i];

                        isMatch = match.Equals(segment, StringComparison.InvariantCultureIgnoreCase) || match == "*";

                        if (!isMatch)
                            break;
                    }

                    if (isMatch)
                        return option.TryCreateResponse();
                }

                if (path.Equals(request.RequestUri.PathAndQuery, StringComparison.InvariantCultureIgnoreCase))
                    return option.TryCreateResponse();
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}