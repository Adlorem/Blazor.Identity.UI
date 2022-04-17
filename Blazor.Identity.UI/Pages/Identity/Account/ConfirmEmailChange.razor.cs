using Blazor.Identity.UI.Common;
using Blazor.Identity.UI.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Blazor.Identity.UI.Pages.Identity.Account
{
    public partial class ConfirmEmailChange
    {
        [Inject]
        NavigationManager _navigationManger { get; set; }
        [Inject]
        UserManager<IdentityUser> _userManager { get; set; }

        private bool _isLoading = true;
        private bool _isEmailConfirmed = false;
        private StringValues userId;
        private StringValues codeId;
        private StringValues newEmail;
        private AlertMessage _alertMessage = AlertMessage.Hide();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var uri = _navigationManger.ToAbsoluteUri(_navigationManger.Uri);

                QueryHelpers.ParseQuery(uri.Query).TryGetValue("user", out userId);
                QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out codeId);
                QueryHelpers.ParseQuery(uri.Query).TryGetValue("email", out newEmail);

                if (string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newEmail))
                {
                    _alertMessage = AlertMessage.Show(AlertType.AlertDanger, "Error"
                        , new[] { "WeCouldNotProcessThisRequest" });
                }
                else
                {
                    var result = await ProcessEmailChangeAsync(userId.ToString(), newEmail.ToString(), codeId.ToString());

                    if (result.Succeeded)
                    {
                        _alertMessage = AlertMessage.Show(AlertType.AlertSuccess, "Congratulations"
                            , new[] { "YouHaveSuccessfullyChangedYourEmail" });
                        _isEmailConfirmed = true;
                    }
                    else
                    {
                        _alertMessage = AlertMessage.Show(AlertType.AlertDanger, result.Errors.Select(x => x.Description));
                    }
                }

                _isLoading = false;
                StateHasChanged();
            }
        }

        private async Task<IdentityResult> ProcessEmailChangeAsync(string userId, string email, string token)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError
            {
                Code = nameof(userId),
                Description = "WeCouldNotProcessThisRequest",
            });

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ChangeEmailAsync(user, email, token);
            await _userManager.SetUserNameAsync(user, email);

            return result;
        }
    }
}
