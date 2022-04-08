using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Identity.UI.Models
{
    public class LoginModel
    {
        public string Email { get; internal set; }
        public string Password { get; internal set; }
        public bool RememberMe { get; internal set; } = false;
        public string ReturnUrl { get; internal set; } = "/";
    }
}
