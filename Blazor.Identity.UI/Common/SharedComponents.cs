using Blazor.Identity.UI.Pages.Identity.Account;
using Blazor.Identity.UI.Pages.Identity.Account.Manage;

using System.Reflection;

namespace Blazor.Identity.UI.Common
{
    public static class SharedComponents
    {
        private static List<Assembly> _assemblies;

        /// <summary>
        /// Gets all additiona assebmlies to support main assembly dapper components.
        /// Suprisingly actually you need to add only one to get all working.
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAll()
        {
            if (_assemblies == null)
            {
                _assemblies = new List<Assembly>{
                typeof(Login).Assembly,
              };
            }

            return _assemblies;
        }
    }
}
