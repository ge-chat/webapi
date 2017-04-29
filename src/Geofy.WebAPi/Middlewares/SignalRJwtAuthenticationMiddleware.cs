using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Linq;

namespace Geofy.WebAPi.Middlewares
{
    public class SignalRJwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public SignalRJwtAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
            {
                if (context.Request.QueryString.HasValue)
                {
                    var token = context.Request.QueryString.Value
                        .Split('&')
                        .SingleOrDefault(x => x.Contains("authorization"))?.Split('=')[1];

                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                    }
                }
            }
            return _next.Invoke(context);
        }
    }
}