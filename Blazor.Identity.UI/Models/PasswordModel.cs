using System.ComponentModel.DataAnnotations;

namespace Blazor.Identity.UI.Models
{
    public class PasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
