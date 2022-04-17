using Blazor.Identity.UI.Common;
using Blazor.Identity.UI.Enums;
using Blazor.Identity.UI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Identity.UI.Components
{
    public partial class UserPersonalDataForm
    {
        [Inject]
        private IStringLocalizer<Language> _localizer { get; set; }
        [Inject]
        private UserManager<IdentityUser> _userManager { get; set; }
        [Inject]
        private IHttpContextAccessor _context { get; set; }

        private AlertMessage _alertMessage = AlertMessage.Hide();
        private UserDetailsModel _model = new UserDetailsModel();
        private bool _isLoading = false;
        private IdentityUser _user;

        protected override async Task OnInitializedAsync()
        {
            var id = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(id);
            
            _model.PhoneNumber = user.PhoneNumber;
            _user = user;
        }
        private async void OnValidSubmit()
        {
            _isLoading = true;

            try
            {
                var id = _context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(id);
                var result = await _userManager.SetPhoneNumberAsync(user, _model.PhoneNumber);
                if (result.Succeeded)
                {
                    _alertMessage = AlertMessage.Show(AlertType.AlertSuccess
                        , new[] { _localizer["YouHaveSuccessfullySetEmail"].Value });
                }
                else
                {
                    _alertMessage = AlertMessage.Show(AlertType.AlertDanger, 
                        result.Errors.Select( x => x.Description).ToArray() );
                }

            }
            catch (Exception)
            {
                _alertMessage = AlertMessage.ShowDefaultError();
            }

            _isLoading = false;
            StateHasChanged();

        }
    }
}
