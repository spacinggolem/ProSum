using System;
using System.Collections.Generic;

namespace ProSum.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value.Length > 0)
                {
                    title = value;
                }
            }

        }
        private DateTime deadline;
        public DateTime Deadline
        {
            get
            {
                return deadline;
            }
            set
            {
                if (value != null)
                {
                    if (value > DateTime.Now)
                    {
                        deadline = value;
                    }

                }
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public List<ProjectStep> Steps { get; set; }
        public Guid ClientId { get; set; }
        public List<User> ProjectManagers { get; set; }

        public Client Client { get; set; }
        public Project() { }

        public Project(string title, DateTime deadline, string description, Guid clientId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Deadline = deadline;
            Description = description;
            ClientId = clientId;
            ProjectManagers = new List<User>();
        }
    }
}
