using System;
using System.Collections.Generic;

namespace ProSum.Models.ViewModels
{
    public class PermissionViewModel
    {
        public List<StepPermission> Permissions { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public bool IsProjectLeader { get; set; }

        public ProjectPermission CurrentPermissions { get; set; }
    }
}
