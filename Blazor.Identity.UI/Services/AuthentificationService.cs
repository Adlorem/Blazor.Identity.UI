using Blazor.Identity.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace Blazor.Identity.UI.Services
{
    public class AuthentificationService
    {
        public static IDictionary<Guid, LoginModel> Logins { get; private set; }
           = new ConcurrentDictionary<Guid, LoginModel>();

        private readonly RequestDelegate _next;

        public AuthentificationService(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, SignInManager<IdentityUser> signInManager)
        {
            if (context.Request.Path == "/Account/Login" && context.Request.Query.ContainsKey("key"))
            {
                var key = Guid.Parse(context.Request.Query["key"]);
                var login = Logins[key];

                var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, login.RememberMe, lockoutOnFailure: true);
                login.Password = string.Empty;
                if (result.Succeeded)
                {
                    Logins.Remove(key);
                    context.Response.Redirect(login.ReturnUrl);
                    return;
                }
                if (result.RequiresTwoFactor)
                {
                    context.Response.Redirect("/Account/LoginWith2fa/" + key);
                    return;
                }
                if (result.IsLockedOut)
                {
                    context.Response.Redirect("/Account/Lockout");
                    return;
                }
                if (result.IsNotAllowed)
                {
                    context.Response.Redirect("/Account/AccessDenied");
                    return;
                }
            }
            else if (context.Request.Path == "/Account/Logout")
            {
                await signInManager.SignOutAsync();
                context.Response.Redirect("/");
                return;
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
