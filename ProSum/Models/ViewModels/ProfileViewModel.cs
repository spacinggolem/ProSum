using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSum.Models.ViewModels
{
    public class ProfileViewModel
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public int ProjectCount { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public List<LogEntryViewModel> logEntries { get; set; }
        public bool Editable { get; set; }
    }
}
