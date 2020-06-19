using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProSum.Models.ViewModels
{
    public class EditProjectViewModel
    {

        public Guid Id { get; set; }
        [Required(ErrorMessage = "Voer een titel in.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Voer een omschrijving in.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Voer een geldige deadline in.")]
        [DataType(DataType.DateTime)]
        public DateTime DeadLine { get; set; }
        [Required(ErrorMessage = "Voer een klant in.")]
        public Guid Client { get; set; }
        
        public List<Client> Clients { get; set; }
    }
}
