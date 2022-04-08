using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Identity.UI.Common
{
    public class ActionResult
    {
        private static readonly ActionResult _success = new ActionResult(true);

        public ActionResult(params string[] errors) : this((IEnumerable<string>)errors)
        {
        }

        public ActionResult(string redirectUrl)
        {
            Redirect = true;
            RedirectUrl = redirectUrl;
        }

        public ActionResult(IEnumerable<string> errors)
        {
            if (errors == null)
            {
                errors = new[] { "Unexpected error" };
            }
            Succeeded = false;
            Errors = errors;
        }

        protected ActionResult(bool success)
        {
            Succeeded = success;
            Errors = new string[0];
        }

        public bool Succeeded { get; private set; } = false;

        public bool Redirect { get; private set; } = false;

        public IEnumerable<string> Errors { get; private set; }

        public string RedirectUrl { get; private set; }

        public static ActionResult Success
        {
            get { return _success; }
        }

        public static ActionResult SetRedirect(string redirectUrl)
        {
            return new ActionResult(redirectUrl);
        }

        public static ActionResult Failed(params string[] errors)
        {
            return new ActionResult(errors);
        }
    }
}
