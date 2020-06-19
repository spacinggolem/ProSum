using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ProSum.Models.ViewModels
{
    public class EditProfileViewModel
    {

        public Guid Id { get; set; }
        [Required(ErrorMessage = "Voer een voornaam in.")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Voer een achternaam in.")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Voer een e-mailadres in.")]
        [EmailAddress(ErrorMessage = "Voer een gelding e-mailadres in.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Voer een telefoonnummer in.")]
        public string Phonenumber { get; set; }
    }
}
