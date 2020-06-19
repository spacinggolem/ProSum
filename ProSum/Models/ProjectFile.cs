using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSum.Models
{
    public class ProjectFile
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid ProjectId { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public DepartmentEnum Department { get; set; }

        public ProjectFile()
        {

        }

        public ProjectFile(Guid accountId, Guid projectId, string link, string title, DepartmentEnum department)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            ProjectId = projectId;
            Link = link;
            Title = title;
            Department = department;
        }
    }
}
