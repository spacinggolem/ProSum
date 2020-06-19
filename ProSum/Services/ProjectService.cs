using Microsoft.Data.SqlClient;
using ProSum.Containers;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProSum.Services
{
    public class ProjectService : IProjectService
    {
        private readonly StepContainer _stepContainer;
        private readonly PermissionContainer _permissionContainer;
        private readonly string ConnectionString;

        public ProjectService(StepContainer stepContainer, PermissionContainer permissionContainer, string connectionString)
        {
            _stepContainer = stepContainer;
            _permissionContainer = permissionContainer;
            ConnectionString = connectionString;
        }

        public void Create(Project project)
        {
            string query = $"INSERT INTO Project (Id, Title, Description, Deadline, ClientId) " +
                $"VALUES ('{project.Id}', '{project.Title}', '{project.Description}', '{project.Deadline.ToString("dd/MMMM/yyyy")}', '{project.ClientId}')";

            string query2 = $"INSERT INTO dbo.ProjectEmployee (AccountId, ProjectId, PermissionId)" +
                $"VALUES ('{project.ProjectManagers[0].Id}', '{project.Id}', '{_permissionContainer.Permissions.FirstOrDefault(p => p.Name == "ProjectManager").Id}')";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);
                SqlCommand q2 = new SqlCommand(query2, conn);

                q.ExecuteNonQuery();
                q2.ExecuteNonQuery();


            }
        }

        public Project Get(Guid id)
        {
            string query = $"SELECT * " +
                $"FROM Project " +
                $"WHERE Id = '{id}'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                if(table.Rows.Count > 0)
                {
                    DataRowParser parser = new DataRowParser(table.Rows[0]);
                    Project project = parser.ParseObject<Project>();

                    SetProjectSteps(project);
                    SetClient(project);
                    SetProjectManagers(project);

                    return project;
                }
                return null;
            }
        }

        public List<Project> Get()
        {
            List<Project> result = new List<Project>();

            string query = $"SELECT * " +
                $"FROM Project";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    Project project = parser.ParseObject<Project>();
                    SetProjectSteps(project);
                    SetClient(project);
                    SetProjectManagers(project);
                    result.Add(project);
                }

                return result;
            }
        }

        public void Edit(Guid guidToUpdate, Project project)
        {
            string query = $"UPDATE Project " +
                $"SET Title='{project.Title}', Description='{project.Description}', Deadline='{project.Deadline.ToString("s")}'" +
                $"WHERE Id='{project.Id}'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);
                q.ExecuteNonQuery();
            }
        }

        public void UpdateProjectStatus(Guid projectId, ProjectStep newStep)
        {
            string query = $"UPDATE dbo.ProjectSteps " +
                $"SET StepStatus = '{newStep.Status.ToString()}' " +
                $"WHERE ProjectId = '{projectId}' AND StepId = '{newStep.Step.Id}'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);

                // Result holds the amount of edited lines
                int result = 0;

                result = q.ExecuteNonQuery();

                if (result == 0)
                {
                    query = $"INSERT INTO dbo.ProjectSteps " +
                    $"(ProjectId, StepId, StepStatus) " +
                    $"VALUES('{projectId}', '{newStep.Step.Id}', '{newStep.Status.ToString()}')";

                    q = new SqlCommand(query, conn);
                    q.ExecuteNonQuery();
                }
            }
        }

        public void Delete(Guid id)
        {
            string query = $"DELETE " +
                $"FROM Project " +
                $"WHERE Id='{id}'";

            string query2 = $"DELETE " +
                $"FROM dbo.ProjectEmployee " +
                $"WHERE projectId = '{id}'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);
                q.ExecuteNonQuery();
            }

        }

        private void SetProjectSteps(Project project)
        {
            List<ProjectStep> result = new List<ProjectStep>();

            string query = $"SELECT * FROM dbo.ProjectSteps WHERE ProjectId = '{project.Id}'";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(query, conn);

                List<Step> AllSteps = new List<Step>(_stepContainer.Steps);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    ProjectStep ps = parser.ParseObject<ProjectStep>();
                    Step.Status status;
                    Enum.TryParse<Step.Status>(parser.GetField<string>("StepStatus"), out status);
                    ps.Status = status;
                    ps.Step = _stepContainer.Steps.FirstOrDefault(s => s.Id == Guid.Parse(parser.GetField<string>("StepId")));
                    result.Add(ps);
                }

                foreach (Step step in AllSteps)
                {
                    ProjectStep projectStep = result.Find(projectStep => projectStep.Step.StepNumber == step.StepNumber);
                    if (projectStep == null)
                    {
                        result.Add(new ProjectStep()
                        {
                            Step = step,
                            Status = Step.Status.not_started
                        });
                    }
                }

                project.Steps = result;
            }
        }

        private void SetClient(Project project)
        {
            string query = $"SELECT * FROM dbo.Client " +
                $"WHERE Id = '{project.ClientId}'";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(q);
                DataTable table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    DataRowParser parser = new DataRowParser(table.Rows[0]);
                    project.Client = parser.ParseObject<Client>();
                }
            }
        }
        public void SetProjectManagers(Project project)
        {
            List<User> result = new List<User>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = $"SELECT dbo.Account.* FROM dbo.ProjectEmployee, dbo.Account WHERE PermissionId = '{_permissionContainer.Permissions.FirstOrDefault(p => p.Name == "ProjectManager").Id}' AND ProjectId = '{project.Id}' AND dbo.Account.Id = dbo.ProjectEmployee.AccountId";
                SqlCommand command = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    result.Add(parser.ParseObject<User>());
                }

            }

            project.ProjectManagers = result;
        }

        public void GetProjectPermissions(User user)
        {
            if (user.Role.Name != RolesEnum.Admin.ToString())
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("select * from dbo.ProjectEmployee where AccountId = @AccountId", conn);
                    command.Parameters.AddWithValue("@AccountId", user.Id);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            Project project = Get(Guid.Parse(row.Field<string>("ProjectId")));
                            Permission permission = _permissionContainer.Permissions.FirstOrDefault(p => p.Id == Guid.Parse(row.Field<string>("PermissionId")));

                            if (project != null && permission != null)
                            {
                                user.AddPermission(project, permission);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Project project in Get())
                {
                    foreach (Permission permission in _permissionContainer.Permissions)
                    {
                        user.AddPermission(project, permission);
                    }
                }
            }
        }

        public void AddProjectPermission(Project project, User user, Permission permission)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("INSERT INTO dbo.ProjectEmployee (AccountId, PermissionId, ProjectId) VALUES (@AccountId, @PermissionId, @ProjectId)", conn);
                command.Parameters.AddWithValue("@AccountId", user.Id);
                command.Parameters.AddWithValue("@PermissionId", permission.Id);
                command.Parameters.AddWithValue("@ProjectId", project.Id);

                command.ExecuteNonQuery();
            }
        }

        public void RemoveProjectPermission(Project project, User user, Permission permission)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("DELETE FROM dbo.ProjectEmployee WHERE AccountId = @AccountId AND PermissionId = @PermissionId AND ProjectId = @ProjectId", conn);
                command.Parameters.AddWithValue("@AccountId", user.Id);
                command.Parameters.AddWithValue("@PermissionId", permission.Id);
                command.Parameters.AddWithValue("@ProjectId", project.Id);

                command.ExecuteNonQuery();
            }
        }

        public List<User> GetAllUsersInProject(Guid project)
        {
            List<User> users = new List<User>();
            string q = $"SELECT * " +
                $"FROM Account " +
                $"WHERE Id IN " +
                    $"(SELECT AccountId " +
                    $"FROM ProjectEmployee " +
                    $"WHERE ProjectId = @ProjectId)";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(q, conn);
                command.Parameters.AddWithValue("@ProjectId", project.ToString());

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    users.Add(parser.ParseObject<User>());
                }


            }

            return users;
        }

        public List<User> GetAllProjectManagersInProject(Guid project)
        {
            List<User> projectManagers = new List<User>();
            string q = $"SELECT * " +
                $"FROM Account " +
                $"WHERE Id IN " +
                    $"(SELECT AccountId " +
                    $"FROM ProjectEmployee " +
                    $"WHERE ProjectId = @ProjectId" +
                    $" AND PermissionId = (" +
                    $"SELECT Id FROM Permission" +
                    $" WHERE Name = 'ProjectManager'))";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(q, conn);
                command.Parameters.AddWithValue("@ProjectId", project.ToString());

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    projectManagers.Add(parser.ParseObject<User>());
                }


            }

            return projectManagers;
        }


        public int GetProjectCount(Guid userid)
        {
            string query = $"SELECT DISTINCT ProjectId FROM dbo.ProjectEmployee WHERE AccountId = @AccountId";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@AccountId", userid);


                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);



                int projectcount = table.Rows.Count;

                return projectcount;
            }

                



        }
    }
}
