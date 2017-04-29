using Geofy.WebAPi.Middlewares;
using Microsoft.AspNet.Builder;

namespace Geofy.WebAPi.Extensions
{
    public static class SignalRJwtAuthenticationExtension
    {
        public static IApplicationBuilder UseSignalRJwtAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SignalRJwtAuthenticationMiddleware>();
        }
    }
}