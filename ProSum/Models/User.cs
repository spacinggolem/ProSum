using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace ProSum.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        private Guid id;
        public Guid Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        private List<ProjectPermission> projectPermissions;
        public List<ProjectPermission> SetPermissions
        {
            set
            {
                projectPermissions = value;
            }
        }
        public IReadOnlyList<ProjectPermission> GetPermissions
        {
            get
            {
                return projectPermissions;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value.Length > 0)
                {
                    name = value;
                }
            }
        }

        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 0)
                {
                    try
                    {
                        email = new MailAddress(value).ToString();
                    }
                    catch (FormatException)
                    {

                        throw new FormatException("Invalid email address");
                    }

                }
                else email = "";
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 0)
                {
                    password = value;
                }
                else password = "";
            }
        }

        private Role role;
        public Role Role
        {
            get
            {
                return role;
            }
            set
            {
                if (role == null)
                {
                    role = value;
                }
            }
        }
        public Guid roleId;

        private string phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                if (value.Length > 0)
                {
                    //if (Regex.Match(value, @"^(([+][(]?[0-9]{1,3}[)]?)|([(]?[0-9]{4}[)]?))\s*[)]?[-\s\.]?[(]?[0-9]{1,3}[)]?([-\s\.]?[0-9]{3})([-\s\.]?[0-9]{3,4})$/g").Success)
                    //{
                    phoneNumber = value;
                    //}
                    //else
                    //{
                    //    throw new ArgumentException("Invalid phoneNumber format");
                    //} 
                }
            }
        }
        public DepartmentEnum Department { get; set; }

        public User()
        {
            projectPermissions = new List<ProjectPermission>();
        }

        public void AddProject(Project project)
        {
            projectPermissions.Add(new ProjectPermission(project));
        }

        public void AddPermission(Project project, Permission permission)
        {
            if (!projectPermissions.Any(p => p.Project.Id == project.Id))
            {
                AddProject(project);
            }
            projectPermissions.FirstOrDefault(p => p.Project.Id == project.Id).AddPermission(permission);
        }

        public void RemovePermission(Project project, Permission permission)
        {
            if (!projectPermissions.Any(p => p.Project.Id == project.Id))
            {
                projectPermissions.FirstOrDefault(p => p.Project == project).RemovePermission(permission);
            }
        }

        public bool HasPermission(Project project, Permission permission)
        {
            if (permission.Name == "ProjectEmployee")
            {
                return projectPermissions.FirstOrDefault(p => p.Project.Id == project.Id) != null &&
                    (
                    projectPermissions.FirstOrDefault(p => p.Project.Id == project.Id).HasPermission(permission.Name) ||
                    projectPermissions.FirstOrDefault(p => p.Project.Id == project.Id).HasPermission("ProjectManager")
                    );
            }
            else
            {
                return projectPermissions.FirstOrDefault(p => p.Project.Id == project.Id) != null && 
                       projectPermissions.FirstOrDefault(p => p.Project.Id == project.Id).HasPermission(permission.Name);
            }
        }

        public ProjectPermission GetProjectPermission(Guid projectId)
        {
            return projectPermissions.FirstOrDefault(p => p.Project.Id == projectId);
        }

        public User(string firstname, string lastname, string name, string password, string email, string phoneNumber, Role role, DepartmentEnum department)
        {
            id = Guid.NewGuid();
            FirstName = firstname;
            LastName = lastname;
            Role = role;
            Name = name;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            projectPermissions = new List<ProjectPermission>();
            Department = department;
        }

        public User(string firstname, string lastname, string name, string password, string email, string phoneNumber, Role role)
        {
            id = Guid.NewGuid();
            FirstName = firstname;
            LastName = lastname;
            Role = role;
            Name = name;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;
            projectPermissions = new List<ProjectPermission>();
        }

        public void Update(string firstname, string lastname, string name, string password, string email, string phoneNumber, Role role, DepartmentEnum department)
        {
            FirstName = firstname;
            LastName = lastname;
            this.role = role;
            this.name = name;
            this.password = password;
            this.email = email;
            this.phoneNumber = phoneNumber;
            Department = department;
        }

    }
    public enum DepartmentEnum
    {
        None,
        Embedded,
        Software,
        Hardware,
        Finance,
        Testing
    }
}
