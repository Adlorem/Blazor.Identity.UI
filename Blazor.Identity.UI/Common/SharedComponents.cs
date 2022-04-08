using Blazor.Identity.UI.Pages.Identity.Account;
using Blazor.Identity.UI.Pages.Identity.Account.Manage;

using System.Reflection;

namespace Blazor.Identity.UI.Common
{
    public static class SharedComponents
    {
        private static List<Assembly> _assemblies;

        public static List<Assembly> GetAll()
        {
            if (_assemblies == null)
            {
                _assemblies = new List<Assembly>{
                typeof(Login).Assembly,
                typeof(Register).Assembly,
                typeof(ConfirmEmail).Assembly,
              };
            }

            return _assemblies;
        }
    }
}
