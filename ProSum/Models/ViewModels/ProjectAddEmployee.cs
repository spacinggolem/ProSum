using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProSum.Models.ViewModels
{
    public class ProjectAddEmployee
    {
        public List<User> Employees { get; set; }

        [Required]

        public Guid ProjectId { get; set; }
        public List<User> ProjectEmployees { get; set; }

        public Guid NewEmployeeId { get; set; }
        public bool IsProjectLeader { get; set; }
    }
}
