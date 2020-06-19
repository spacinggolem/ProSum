using System.Collections.Generic;
using System.Linq;

namespace ProSum.Models
{
    public class ProjectPermission
    {
        private Project project;
        public Project Project { 
            get 
            {
                return this.project;
            } 
        }
        private List<Permission> permissions;
        public IReadOnlyList<Permission> Permissions { 
            get
            {
                return permissions;
            } 
        }

        public ProjectPermission(Project project)
        {
            permissions = new List<Permission>();
            this.project = project;
        }

        public void AddPermission(Permission permission)
        {
            permissions.Add(permission);
        }

        public void RemovePermission(Permission permission)
        {
            permissions.Remove(permission);
        }

        public bool HasPermission(string name)
        {
            return permissions.FindAll(p => p.Name == name).Any();
        }
    }
}
