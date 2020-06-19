using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSum.Models.ViewModels
{
    public class CreateFileViewModel
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public Guid ProjectId { get; set; }
        public DepartmentEnum Department { get; set; }
    }
}
