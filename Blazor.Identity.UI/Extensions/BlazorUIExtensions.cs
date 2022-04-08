using Blazor.Identity.UI.Common;
using Blazor.Identity.UI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Identity.UI.Extensions
{
    public static class BlazorUIExtensions
    {
        public static IApplicationBuilder UseBlazorIdentityUI(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AuthentificationService>();

            return builder;
        }
    }
}
