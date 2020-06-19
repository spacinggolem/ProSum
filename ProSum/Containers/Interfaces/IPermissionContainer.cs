using ProSum.Models;
using System;
using System.Collections.Generic;

namespace ProSum.Containers.Interfaces
{
    public interface IPermissionContainer
    {
        public IReadOnlyList<Permission> Permissions { get; }
        public void CreatePermission(string name);
        public void DeletePermission(Permission permission);
        public void UpdatePermissionName(string newName, string oldName);
    }
}
