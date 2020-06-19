using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProSum.Containers
{
    class LoggingContainer : ILogger
    {
        private IStepContainer _StepContainer;
        private string ConnectionString;

        public LoggingContainer(IStepContainer stepContainer, string connectionString)
        {
            _StepContainer = stepContainer;
            ConnectionString = connectionString;
        }

        public List<LogEntry> GetLogs()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                List<LogEntry> logEntries = new List<LogEntry>();
                
                string q = $"SELECT * FROM dbo.ProjectLogs";

                SqlCommand command = new SqlCommand(q, conn);
                command.Prepare();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach(DataRow row in dt.Rows)
                {
                    DataRowParser rowParser = new DataRowParser(row);
                    LogEntry entry = rowParser.ParseObject<LogEntry>();
                    Step step = _StepContainer.Steps.FirstOrDefault(Step => Step.Id == entry.StepId);
                    if(step != null)
                    {
                        entry.Step = step;
                    }
                    logEntries.Add(entry);

                }
                
                return logEntries;
            }
        }

        public List<LogEntry> GetProjectLog(Guid projectId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                List<LogEntry> logEntries = new List<LogEntry>();

                string q = $"SELECT * FROM dbo.ProjectLogs" +
                    $"WHERE ProjectId = '{projectId}'";

                SqlCommand command = new SqlCommand(q, conn);
                command.Prepare();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    DataRowParser rowParser = new DataRowParser(row);
                    LogEntry entry = rowParser.ParseObject<LogEntry>();
                    Step step = _StepContainer.Steps.FirstOrDefault(Step => Step.Id == entry.StepId);
                    if (step != null)
                    {
                        entry.Step = step;
                    }
                    logEntries.Add(entry);
                }
                
                return logEntries;
            }
        }

        public List<LogEntry> GetUpdateTypeLog(LogEntryUpdateType updateType)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                List<LogEntry> logEntries = new List<LogEntry>();

                string q = $"SELECT * FROM dbo.ProjectLogs" +
                    $"WHERE UpdateType LIKE '{updateType.ToString()}'";

                SqlCommand command = new SqlCommand(q, conn);
                command.Prepare();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    DataRowParser rowParser = new DataRowParser(row);
                    LogEntry entry = rowParser.ParseObject<LogEntry>();
                    Step step = _StepContainer.Steps.FirstOrDefault(Step => Step.Id == entry.StepId);
                    if (step != null)
                    {
                        entry.Step = step;
                    }
                    logEntries.Add(entry);
                }
                
                return logEntries;
            }
        }

        public List<LogEntry> GetUserLog(Guid userId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                List<LogEntry> logEntries = new List<LogEntry>();

                string q = $"SELECT * FROM dbo.ProjectLogs " +
                    $"WHERE UserId = '{userId.ToString()}'";

                SqlCommand command = new SqlCommand(q, conn);
                command.Prepare();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    DataRowParser rowParser = new DataRowParser(row);
                    LogEntry entry = rowParser.ParseObject<LogEntry>();
                    Step step = _StepContainer.Steps.FirstOrDefault(Step => Step.Id == entry.StepId);
                    if (step != null)
                    {
                        entry.Step = step;
                    }
                    logEntries.Add(entry);
                }
                
                return logEntries;
            }
        }

        public void Log(Guid userId, Guid projectId, Guid stepId, LogEntryUpdateType updateType, Step.Status oldStatus, Step.Status newStatus)
        {
            Step step = _StepContainer.Steps.FirstOrDefault(Step => Step.Id == stepId);
            string q = "";
            if (step != null)
            {
                LogEntry logEntry = new LogEntry(userId, projectId, step, updateType, oldStatus, newStatus);
                q = $"INSERT INTO dbo.ProjectLogs" +
                        $"(Id, UserId, ProjectId, StepId, UpdateType, OldStepStatus, NewStepStatus)" +
                        $"VALUES ('{logEntry.Id}', '{logEntry.UserId}', '{logEntry.ProjectId}', " +
                        $"'{step.Id}', '{logEntry.UpdateType.ToString()}', '{oldStatus.ToString()}', '{newStatus.ToString()}')";
            }
            else
            {
                throw new ArgumentException("Step was not found");
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(q, conn);

                command.Prepare();
                command.ExecuteNonQuery();
                
            }
        }

        public void Log(Guid userId, Guid projectId, LogEntryUpdateType updateType)
        {
            LogEntry logEntry = new LogEntry(userId, projectId, updateType);
            string q = $"INSERT INTO dbo.ProjectLogs" +
                    $"(Id, UserId, ProjectId, UpdateType)" +
                    $"VALUES ('{logEntry.Id}', '{logEntry.UserId}', '{logEntry.ProjectId}', '{logEntry.UpdateType.ToString()}')";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(q, conn);

                command.Prepare();
                command.ExecuteNonQuery();
                
            }
        }

        public void Log(Guid userId, LogEntryUpdateType updateType, Guid updatedUserId)
        {
            LogEntry logEntry = new LogEntry(userId, updateType, updatedUserId);
            string q = $"INSERT INTO dbo.ProjectLogs" +
                    $"(Id, UserId, UpdateType, UpdatedUserId)" +
                    $"VALUES ('{logEntry.Id}', '{logEntry.UserId}', '{logEntry.UpdateType.ToString()}', '{updatedUserId}')";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(q, conn);

                command.Prepare();
                command.ExecuteNonQuery();
                
            }
        }

        public void Log(Guid userId, Guid projectId, LogEntryUpdateType updateType, Guid updatedUserId)
        {
            LogEntry logEntry = new LogEntry(userId, projectId, updateType, updatedUserId);
            string q = $"INSERT INTO dbo.ProjectLogs" +
                    $"(Id, UserId, UpdateType, ProjectId, UpdatedUserId)" +
                    $"VALUES ('{logEntry.Id}', '{logEntry.UserId}', '{logEntry.UpdateType.ToString()}', '{projectId}', '{updatedUserId}')";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(q, conn);

                command.Prepare();
                command.ExecuteNonQuery();
                
            }
        }
    }
}
