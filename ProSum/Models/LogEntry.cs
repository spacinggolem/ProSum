using System;

namespace ProSum.Models
{
    public class LogEntry
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid StepId { get; set; }
        public Step Step { get; set; }
        public DateTime TimeStamp { get; set; }
        public LogEntryUpdateType UpdateType { get; set; }
        public Step.Status OldStepStatus { get; set; }
        public Step.Status NewStepStatus { get; set; }
        public Guid UpdatedUserId { get; set; }

        public LogEntry() { }

        public LogEntry(Guid userId, Guid projectId, Step step, LogEntryUpdateType updateType, Step.Status oldStatus, Step.Status newStatus)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ProjectId = projectId;
            Step = step;
            UpdateType = updateType;
            OldStepStatus = oldStatus;
            NewStepStatus = newStatus;
        }

        public LogEntry(Guid userId, LogEntryUpdateType updateType, Guid updatedUserId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            UpdateType = updateType;
            UpdatedUserId = updatedUserId;
        }

        public LogEntry(Guid userId, Guid projectId, LogEntryUpdateType updateType)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ProjectId = projectId;
            UpdateType = updateType;
        }

        public LogEntry(Guid userId, Guid projectId, LogEntryUpdateType updateType, Guid updatedUserId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ProjectId = projectId;
            UpdateType = updateType;
            UpdatedUserId = updatedUserId;
        }
    }
}
