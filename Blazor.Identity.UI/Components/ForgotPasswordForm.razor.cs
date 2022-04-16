using Blazor.Identity.UI.Common;
using Blazor.Identity.UI.Enums;
using Blazor.Identity.UI.Interfaces;
using Blazor.Identity.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using System.Text;
using System.Text.Encodings.Web;

namespace Blazor.Identity.UI.Components
{
    public partial class ForgotPasswordForm
    {
        [Inject]
        NavigationManager _navigationManager { get; set; }
        [Inject]
        UserManager<IdentityUser> _userManager { get; set; }
        [Inject]
        IStringLocalizer<Language> _localizer { get; set; }
        [Inject]
        IEmailService _emailService { get; set; }

        private ForgotPasswordModel _model = new ForgotPasswordModel();
        private bool _isLoading = false;
        private bool _isResetCompleted = false;
        private AlertMessage _alertMessage = AlertMessage.Hide();
        private AlertMessage _resetCompletedMessage = AlertMessage.Hide();

        private async void OnValidSubmit()
        {
            _isLoading = true;

            var result = await ResetPasswordAsync(_model.Email);

            if (result.Succeeded)
            {
                var test = _localizer["CheckYourInboxToCompletePasswordReset"];
                _isResetCompleted = true;
                _resetCompletedMessage = AlertMessage.Show(AlertType.AlertSuccess
                    ,"Success", new[] { _localizer["CheckYourInboxToCompletePasswordReset"].Value });
            }
            else
            {
                _alertMessage = AlertMessage.Show(AlertType.AlertDanger, result.Errors);
            }

            _isLoading = false;

            StateHasChanged();
        }

        private async Task<ActionResult> ResetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return ActionResult.Failed(new[] { _localizer["UnknownUser"].Value });
            }

            if ((await _userManager.IsEmailConfirmedAsync(user)) == false)
            {
                return ActionResult.Failed(new[] { _localizer["AccountNotActivated"].Value });
            }

            await SendForgotPasswordEmailAsync(user);

            return ActionResult.Success;
        }


        private async Task SendForgotPasswordEmailAsync(IdentityUser user)
        {
            var baseUri = _navigationManager.BaseUri;
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = $"{baseUri}Account/ResetPassword?code={code}&email={user.Email}";
            await _emailService.SendEmailAsync(user.Email, _localizer["ResetYourPassword"].Value ,
                $"{_localizer["ToResetYourPassword"].Value} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["ClickHere"]}</a>.");
        }
    }
}
