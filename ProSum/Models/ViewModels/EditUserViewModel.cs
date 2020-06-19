namespace ProSum.Models.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class EditUserViewModel
    {
        public Guid userId { get; set; }

        [Required(ErrorMessage = "Voer een gebruikersnaam in.")]
        public string Username { get; set;}
        [Required(ErrorMessage = "Voer een voornaam in.")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Voer een achternaam in.")]
        public string Lastname { get; set; }
        [EmailAddress(ErrorMessage = "Voer een geldig e-mailadres in.")]
        [Required(ErrorMessage = "Voer een email in.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Voer een wachtwoord in.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Voer een functie in.")]
        public RolesEnum Role { get; set; }
        [Required(ErrorMessage = "Voer een afdeling in.")]
        public DepartmentEnum Department { get; set; }
        [Required(ErrorMessage = "Voer een telefoonnummer in.")]
        public string PhoneNumber { get; set; }
    }


}
