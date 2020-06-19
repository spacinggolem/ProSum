using ProSum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSum.Containers.Interfaces
{
    public interface IProjectFileContainer
    {
        public List<ProjectFile> GetListForProject(Guid projectId);
        public void CreateProjectFile(ProjectFile file);
    }
}
