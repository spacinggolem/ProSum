using ProSum.Models;
using System;
using System.Collections.Generic;

namespace ProSum.Containers.Interfaces
{
    public interface IAnnouncementContainer
    {
        public List<Announcement> Get(Guid projectId);
        public void AddAnnouncementToDB(Announcement announcement);
    }
}
