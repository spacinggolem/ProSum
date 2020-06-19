using System;
using System.Collections.Generic;

namespace ProSum.Models.ViewModels
{
    public class AnnouncementViewModel
    {
        public Guid ProjectId { get; set; }
        public List<Announcement> Announcements { get; set; }
        public List<ProjectFile> ProjectFiles { get; set; }
    }
}
