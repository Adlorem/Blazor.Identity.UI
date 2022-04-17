using Blazor.Identity.UI.Common;
using Blazor.Identity.UI.Enums;
using Blazor.Identity.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text.Json;

namespace Blazor.Identity.UI.Components
{
    public partial class ManagePersonalDataForm
    {
        [Inject]
        private IStringLocalizer<Language> _localizer { get; set; }
        [Inject]
        private UserManager<IdentityUser> _userManager { get; set; }
        [Inject]
        private IHttpContextAccessor _context { get; set; }
        [Inject]    
        private IJSRuntime _jsRuntime { get; set; }
        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private AlertMessage _alertMessage = AlertMessage.Hide();
        private PasswordModel _model = new PasswordModel();
        private bool _isLoading = false;
        private bool _isConfirmDelete = false;
        private IdentityUser _user;

        private async void OnValidSubmit()
        {
            _isLoading = true;

            try
            {
                var result = await DeleteUserAccountAsync(_model.Password);
                if (result.Succeeded)
                {
                    _navigationManager.NavigateTo($"/Account/Logout?returnUrl=/Account/Deleted", forceLoad: true);
                }
                else
                {
                    _alertMessage = AlertMessage.Show(AlertType.AlertDanger, result.Errors);
                }

            }
            catch (Exception)
            {
                _alertMessage = AlertMessage.ShowDefaultError();
            }

            _isLoading = false;
            StateHasChanged();
        }

        private async Task DownloadPersonalDataAsync()
        {
            var id = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return;

            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(IdentityUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

            var file = JsonSerializer.SerializeToUtf8Bytes(personalData);

            await _jsRuntime.InvokeAsync<object>("SaveAsFile", "PersonalData.json",
            Convert.ToBase64String(file));
        }

        private void ConfirmAccountDelete()
        {
            _isConfirmDelete = true;
            _alertMessage = AlertMessage.Show(AlertType.AlertDanger
                ,new [] { _localizer["DeleteAccountWarning"].Value });
        }

        private async Task<ActionResult> DeleteUserAccountAsync(string password)
        {
            var id = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return ActionResult.Failed(new[] { _localizer["UnknownUser"].Value });
            }

            var isPasswordSet = await _userManager.HasPasswordAsync(user);
            if (isPasswordSet)
            {
                if (!await _userManager.CheckPasswordAsync(user, password))
                {
                    return ActionResult.Failed(new[] { _localizer["IncorrectPassword"].Value });
                }
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return ActionResult.Failed(new[] { _localizer["WeCouldNotProcessThisRequest"].Value });
            }

            return ActionResult.Success;
        }
    }
}
