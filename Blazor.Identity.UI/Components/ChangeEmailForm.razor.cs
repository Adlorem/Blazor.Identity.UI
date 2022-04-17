using Blazor.Identity.UI.Common;
using Blazor.Identity.UI.Enums;
using Blazor.Identity.UI.Interfaces;
using Blazor.Identity.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Blazor.Identity.UI.Components
{
    public partial class ChangeEmailForm
    {
        [Inject]
        private IStringLocalizer<Language> _localizer { get; set; }
        [Inject]
        private UserManager<IdentityUser> _userManager { get; set; }
        [Inject]
        private IHttpContextAccessor _context { get; set; }
        [Inject]
        private NavigationManager _navigationManager { get; set; } 
        [Inject]
        private IEmailService _emailService { get; set; }

        private AlertMessage _alertMessage = AlertMessage.Hide();
        private ChangeEmailModel _model = new ChangeEmailModel();
        private bool _isLoading = false;
        private IdentityUser _user;

        protected override async Task OnInitializedAsync()
        {
            var id = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id);
            _user = user;
        }
        private async void OnValidSubmit()
        {
            _isLoading = true;

            try
            {
                var isEmailRegistered = await _userManager.FindByEmailAsync(_model.NewEmail);
                if (isEmailRegistered != null)
                {
                    _alertMessage = AlertMessage.Show(AlertType.AlertDanger
                        , new[] { _localizer["ThisEmailIsAlreadyTaken"].Value });
                }
                else if (_model.NewEmail != _user.Email)
                {
                    await SendEmailChangeConfirmationAsync(_user, _model.NewEmail);
                    _alertMessage = AlertMessage.Show(AlertType.AlertSuccess
                        , new[] { _localizer["CheckYourInboxToConfirmEmailChange"].Value });
                }
            }
            catch (Exception)
            {
                _alertMessage = AlertMessage.ShowDefaultError();
            }

            _isLoading = false;
            StateHasChanged();
        }

        private async Task SendEmailChangeConfirmationAsync(IdentityUser user, string newEmail)
        {
            var baseUri = _navigationManager.BaseUri;
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = $"{baseUri}Account/ConfirmEmailChange?user={user.Id}&email={newEmail}&code={code}";
            await _emailService.SendEmailAsync(user.Email, _localizer["ConfirmNewEmail"].Value,
                $"{_localizer["PleaseConfirmEmailChange"].Value} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["ClickHere"].Value}</a>.");
        }
    }
}
