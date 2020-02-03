﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Rat.Intercepting
{
    internal static class Extensions
    {
        private static readonly Dictionary<HttpStatusCode, Func<RequestInterceptorOptions, HttpResponseMessage>> ResponseHandlers =
            new Dictionary<HttpStatusCode, Func<RequestInterceptorOptions, HttpResponseMessage>>
            {
                [HttpStatusCode.BadRequest] = x => x.CreateMessage(HttpStatusCode.BadRequest),
                [HttpStatusCode.Unauthorized] = x => x.CreateMessage(HttpStatusCode.Unauthorized),
                [HttpStatusCode.InternalServerError] = x => x.CreateMessage(HttpStatusCode.InternalServerError),
                [HttpStatusCode.BadGateway] = x => x.CreateMessage(HttpStatusCode.BadGateway),
                [HttpStatusCode.GatewayTimeout] = x => x.CreateMessage(HttpStatusCode.BadGateway),

                [HttpStatusCode.OK] = x => x.CreateMessage(HttpStatusCode.OK),
                [HttpStatusCode.Created] = x => x.CreateMessage(HttpStatusCode.Created),
                [HttpStatusCode.NotFound] = x => x.CreateMessage(HttpStatusCode.NotFound)
            };

        internal static HttpResponseMessage TryCreateResponse(this RequestInterceptorOptions options)
        {
            if (options == null)
                return null;

            if (ResponseHandlers.TryGetValue((HttpStatusCode)options.ReturnStatusCode, out var handler))
                return handler(options);

            return null;
        }

        private static HttpResponseMessage CreateMessage(this RequestInterceptorOptions options, HttpStatusCode statusCode)
        {
            var message = new HttpResponseMessage(statusCode);

            if (string.IsNullOrWhiteSpace(options.ReturnJsonContent))
                return message.AddResponseHeaders(options.Headers);

            message.Content = new StringContent(options.ReturnJsonContent, Encoding.UTF8, "application/json");

            return message.AddResponseHeaders(options.Headers);
        }

        private static HttpResponseMessage AddResponseHeaders(this HttpResponseMessage message, Collection<HttpResponseHeader> headers)
        {
            foreach (HttpResponseHeader header in headers)
            {
                message.Headers.Add(header.Name, header.Value);
            }

            return message;
        }
    }
}