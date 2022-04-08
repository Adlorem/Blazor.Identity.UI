using Blazor.Identity.UI.Interfaces;

namespace BlazorIdentityUIExample.Services
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
