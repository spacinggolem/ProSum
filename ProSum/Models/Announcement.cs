using System;

namespace ProSum.Models
{
    public class Announcement
    {
        public Guid AnnouncementId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AuthorId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public User Author { get; set; }
        public Announcement(Guid projectId, Guid userId, string title, string message)
        {
            AnnouncementId = Guid.NewGuid();
            ProjectId = projectId;
            AuthorId = userId;
            Title = title;
            Message = message;
        }

        public Announcement()
        {

        }
    }
}
