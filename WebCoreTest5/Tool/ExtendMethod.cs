using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebCoreTest5
{
    public static class ExtendMethod
    {
        public static void SetHeaderOnStarting(this HttpResponse resp, string name, string value)
        {
            if (resp.HasStarted)
                return;
            resp.OnStarting(() =>
            {
                SetHeader(resp, name, value);
                return Task.CompletedTask;
            });
        }

        public static void SetHeader(this HttpResponse resp, string name, string value)
        {
            var existing = resp.Headers[name].FirstOrDefault();
            if (existing != null && value == null)
                resp.Headers.Remove(name);
            else
                resp.Headers[name] = value;
        }

        public static T GetOrDefault<T>(this IConfiguration configuration, string key, T defaultValue)
        {
            var str = configuration[key] ?? configuration[key.Replace(".", string.Empty, StringComparison.InvariantCulture)];
            if (str == null)
                return defaultValue;
            if (typeof(T) == typeof(bool))
            {
                var trueValues = new[] { "1", "true" };
                var falseValues = new[] { "0", "false" };
                if (trueValues.Contains(str, StringComparer.OrdinalIgnoreCase))
                    return (T)(object)true;
                if (falseValues.Contains(str, StringComparer.OrdinalIgnoreCase))
                    return (T)(object)false;
                throw new FormatException();
            }
            else if (typeof(T) == typeof(Uri))
                if (string.IsNullOrEmpty(str))
                {
                    return defaultValue;
                }
                else
                {
                    return (T)(object)new Uri(str, UriKind.Absolute);
                }
            else if (typeof(T) == typeof(string))
                return (T)(object)str;
            else if (typeof(T) == typeof(IPAddress))
                return (T)(object)IPAddress.Parse(str);
            else if (typeof(T) == typeof(IPEndPoint))
            {
                var separator = str.LastIndexOf(":", StringComparison.InvariantCulture);
                if (separator == -1)
                    throw new FormatException();
                var ip = str.Substring(0, separator);
                var port = str.Substring(separator + 1);
                return (T)(object)new IPEndPoint(IPAddress.Parse(ip), int.Parse(port, CultureInfo.InvariantCulture));
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)int.Parse(str, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new NotSupportedException("Configuration value does not support time " + typeof(T).Name);
            }
        }

        public static Action<StaticFileResponseContext> HandleStaticFileResponse()
        {
            return context =>
            {
                if (context.Context.Request.Query.ContainsKey("download"))
                {
                    context.Context.Response.Headers["Content-Disposition"] = "attachment";
                }
            };
        }

        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var claim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (claim == null || string.IsNullOrEmpty(claim.Value))
                {
                    return 0;
                }

                return int.Parse(claim.Value);
            }
            catch
            {
                return 0;
            }
        }
    }
}
