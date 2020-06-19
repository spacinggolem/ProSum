using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace ProSum.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Voer een e-mailadres in.")]
        [EmailAddress(ErrorMessage = "Voer een geldig e-mailadres in.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Voer een wachtwoord in.")]

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
