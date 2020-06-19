using ProSum.Models;
using System;
using System.Collections.Generic;

namespace ProSum.Containers.Interfaces
{
    public interface ILogger
    {
        void Log(Guid userId, LogEntryUpdateType updateType, Guid updatedUser);

        void Log(Guid userId, Guid projectId, LogEntryUpdateType updateType);
        void Log(Guid userId, Guid projectId, LogEntryUpdateType updateType, Guid updatedUser);
        void Log(Guid userId, Guid projectId, Guid StepId, LogEntryUpdateType updateType, Step.Status oldStatus, Step.Status newStatus);

        List<LogEntry> GetUserLog(Guid userId);

        List<LogEntry> GetProjectLog(Guid projectId);

        List<LogEntry> GetUpdateTypeLog(LogEntryUpdateType updateType);

        List<LogEntry> GetLogs();
    }
}
