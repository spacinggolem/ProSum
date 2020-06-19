using System;

namespace ProSum.Models.ViewModels
{
    public class LogEntryViewModel
    {
        public Project Project { get; set; }
        public User Author { get; set; }
        public User UpdatedUser { get; set; }
        public Step Step { get; set; }
        public Step.Status OldStatus { get; set; }
        public Step.Status NewStatus { get; set; }
        public LogEntryUpdateType UpdateType { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
