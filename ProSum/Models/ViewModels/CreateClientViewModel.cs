using System;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;

namespace ProSum.Models.ViewModels
{
    public class CreateClientViewModel
    {
        [Required(ErrorMessage = "Voer een naam in.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Voer een e-mailadres in.")]
        [EmailAddress(ErrorMessage = "Voer een geldig e-mailadres in.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Voer een bedrijf in.")]
        public string Company { get; set; }
        [Range(0, Int64.MaxValue, ErrorMessage = "Telefoonnummer kan alleen cijfers bevatten.")]
        [StringLength(20, ErrorMessage = "Onjuist formaat voor telefoonnummer, gebruik minstens elf cijfers. (bv. 31612345678 of 3113987654)", MinimumLength = 11)]
        [Required(ErrorMessage = "Voer een telefoonnummer in.")]
        public string PhoneNumber { get; set; }

        public CreateClientViewModel(string name, string email, string company, string phoneNumber)
        {
            Name = name;
            Email = email;
            Company = company;
            PhoneNumber = phoneNumber;
        }


        public CreateClientViewModel()
        {

        }
    }
}
