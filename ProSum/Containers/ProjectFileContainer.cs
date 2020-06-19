using Microsoft.Data.SqlClient;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProSum.Containers
{
    public class ProjectFileContainer : IProjectFileContainer
    {
        private List<ProjectFile> ProjectFiles;
        private readonly string ConnectionString;

        public ProjectFileContainer(string connectionString)
        {
            ConnectionString = connectionString;
            ProjectFiles = GetProjectFiles();
        }

        public List<ProjectFile> GetListForProject(Guid projectId)
        {
            return ProjectFiles.Where(files => files.ProjectId == projectId).ToList();
        }

        private List<ProjectFile> GetProjectFiles()
        {
            List<ProjectFile> files = new List<ProjectFile>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * " +
                    "FROM dbo.ProjectFiles";

                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    ProjectFile file = parser.ParseObject<ProjectFile>();
                    files.Add(file);
                }

                return files;
            }
        }

        public void CreateProjectFile(ProjectFile file)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("INSERT INTO dbo.ProjectFiles (Id, AccountId, ProjectId, Title, Link, Department) " +
                    "VALUES (@id, @accountId, @projectId, @title, @link, @department)", conn);

                SqlParameter idParam = new SqlParameter("@id", SqlDbType.Char, 36);
                idParam.Value = file.Id.ToString();

                SqlParameter accountParam = new SqlParameter("@accountId", SqlDbType.Char, 36);
                accountParam.Value = file.AccountId.ToString();

                SqlParameter projectIdParam = new SqlParameter("@projectId", SqlDbType.Char, 36);
                projectIdParam.Value = file.ProjectId.ToString();

                SqlParameter titleParam = new SqlParameter("@title", SqlDbType.VarChar, 30);
                titleParam.Value = file.Title;

                SqlParameter linkParam = new SqlParameter("@link", SqlDbType.VarChar, 255);
                linkParam.Value = file.Link;

                SqlParameter departmentParam = new SqlParameter("@department", SqlDbType.VarChar, 20);
                departmentParam.Value = file.Department.ToString();

                command.Parameters.Add(idParam);
                command.Parameters.Add(accountParam);
                command.Parameters.Add(projectIdParam);
                command.Parameters.Add(titleParam);
                command.Parameters.Add(linkParam);
                command.Parameters.Add(departmentParam);

                command.Prepare();
                command.ExecuteNonQuery();
                ProjectFiles.Add(file);
            }
        }
    }
}
