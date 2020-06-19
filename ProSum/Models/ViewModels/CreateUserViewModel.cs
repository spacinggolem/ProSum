namespace ProSum.Models.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class CreateUserViewModel
    {
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
        [Range(0, Int64.MaxValue, ErrorMessage = "Telefoonnummer kan alleen cijfers bevatten.")]
        [StringLength(20,ErrorMessage = "Onjuist formaat voor telefoonnummer, gebruik minstens elf cijfers. (bv. 31612345678 of 3113987654)", MinimumLength = 11)]
        [Required(ErrorMessage = "Voer een telefoonnummer in.")]
        public string PhoneNumber { get; set; }
    }


}
