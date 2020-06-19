using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProSum.Models.ViewModels
{
    public class CreateProjectViewModel
    {
        public List<Client> Clients { get; set; }
        [Required(ErrorMessage = "Voer een titel in.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Voer een omschrijving in.")]
        public string Description { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Voer een geldige datum in.")]
        public DateTime DeadLine { get; set; }

        public Guid Client { get; set; }
    }
}
