using ProSum.Models;
using System;
using System.Collections.Generic;

namespace ProSum.Services.Interfaces
{
    public interface IProjectService
    {
        void Create(Project project);

        Project Get(Guid id);

        List<Project> Get();

        void Edit(Guid guidToUpdate, Project project);

        void UpdateProjectStatus(Guid projectId, ProjectStep newStep);

        void Delete(Guid id);

        void GetProjectPermissions(User user);

        void AddProjectPermission(Project project, User user, Permission permission);

        void RemoveProjectPermission(Project project, User user, Permission permission);

        List<User> GetAllUsersInProject(Guid project);
        List<User> GetAllProjectManagersInProject(Guid project);
        int GetProjectCount(Guid userid);
    }
}
