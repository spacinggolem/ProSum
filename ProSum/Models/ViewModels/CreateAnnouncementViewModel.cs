using System;
using System.ComponentModel.DataAnnotations;

namespace ProSum.Models.ViewModels
{
    public class CreateAnnouncementViewModel
    {
        public Guid ProjectId { get; set; }
        [Required(ErrorMessage = "Voer een titel in.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Voer een bericht in.")]
        public string Message { get; set; }
    }
}
