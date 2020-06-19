using System;
using System.Collections.Generic;

namespace ProSum.Models.ViewModels
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<User> ProjectManagers { get; set; }
        public DateTime DeadLine { get; set; }
        public List<ProjectStep> Steps { get; set; }
        public Client Client { get; set; }
        public List<Permission> UserPermissions { get; set; }
        public List<User> ProjectEmployees { get; set; }
        public List<Announcement> Announcements { get; set; }
    }
}
