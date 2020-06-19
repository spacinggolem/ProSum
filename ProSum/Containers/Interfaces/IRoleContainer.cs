using ProSum.Models;
using System.Collections.Generic;

namespace ProSum.Containers.Interfaces
{
    public interface IRoleContainer
    {
        public IReadOnlyList<Role> Roles { get; }
        public void CreateRole(string name);
        public void DeleteRole(Role role);
    }
}
