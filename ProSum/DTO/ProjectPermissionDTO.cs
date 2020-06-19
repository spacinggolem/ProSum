using System;

namespace ProSum.DTO
{
    public class ProjectPermissionDTO
    {
        public Guid AccountId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
