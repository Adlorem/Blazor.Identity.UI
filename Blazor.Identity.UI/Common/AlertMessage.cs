using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazor.Identity.UI.Enums;

namespace Blazor.Identity.UI.Common
{
    public class AlertMessage
    {
        public AlertMessage(string alertType, string title, IEnumerable<string> messages) : this((IEnumerable<string>)messages)
        {
            AlertType = alertType;
            Title = title;
            IsVisible = true;
        }

        public AlertMessage(bool isVisible)
        {
            IsVisible = isVisible;
        }

        public AlertMessage(IEnumerable<string> messages)
        {
            IsVisible = true;
            Messages = messages;
        }

        public string AlertType { get; private set; }

        public bool IsVisible { get; private set; } = false;

        public string Title { get; private set; }

        public IEnumerable<string> Messages { get; private set; }

        public static AlertMessage Show (string alertType, IEnumerable<string> messages)
        {
            return new AlertMessage(alertType, string.Empty, messages);
        }

        public static AlertMessage Show(string alertType, string title, IEnumerable<string> messages)
        {
            return new AlertMessage(alertType, title, messages);
        }

        public static AlertMessage Hide ()
        {
            return new AlertMessage(false);
        }

    }
}
