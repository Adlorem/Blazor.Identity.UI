using Blazor.Identity.UI.Common;
using Blazor.Identity.UI.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Identity.UI.Pages.Identity.Account
{
    public partial class ConfirmEmail
    {
        [Inject]
        NavigationManager _navigationManger { get; set; }
        [Inject]
        UserManager<IdentityUser> _userManager { get; set; }

        private bool _isLoading = true;
        private bool _isEmailConfirmed = false;
        private StringValues userId;
        private StringValues codeId;
        private AlertMessage _alertMessage = AlertMessage.Hide();

        protected override async void OnInitialized()
        {

            var uri = _navigationManger.ToAbsoluteUri(_navigationManger.Uri);

            QueryHelpers.ParseQuery(uri.Query).TryGetValue("user", out userId);
            QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out codeId);

            if (string.IsNullOrEmpty(userId.ToString()) || string.IsNullOrEmpty(userId))
            {
                _alertMessage = AlertMessage.Show(AlertType.AlertDanger, "Error"
                    , new[] { "WeCouldNotProcessThisRequest" });
            }
            else
            {
                var result = await ProcessEmailConfirmation(userId.ToString(), codeId.ToString());

                if (result.Succeeded)
                {
                    _alertMessage = AlertMessage.Show(AlertType.AlertSuccess, "Congratulations"
                        , new[] { "YouHaveSuccessfullyConfirmedEmail" });
                    _isEmailConfirmed = true;
                }
                else if (result.Errors is not null)
                {
                    _alertMessage = AlertMessage.Show(AlertType.AlertDanger, result.Errors.Select(x => x.Description));
                }
            }

            _isLoading = false;
            StateHasChanged();
        }

        private async Task<IdentityResult> ProcessEmailConfirmation(string userId, string token)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError
            {
                Code = nameof(userId),
                Description = "WeCouldNotProcessThisRequest",
            });

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result;
        }
    }
}
