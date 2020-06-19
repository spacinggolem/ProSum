using Microsoft.Data.SqlClient;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace ProSum.Containers
{
    public class RoleContainer : IRoleContainer
    {
        private List<Role> roles;
        public IReadOnlyList<Role> Roles
        {
            get
            {
                return roles;
            }
        }
        private string ConnectionString;

        public RoleContainer(string connectionString)
        {
            ConnectionString = connectionString;
            roles = new List<Role>();
            GetStoredRoles();
        }

        private void GetStoredRoles()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.Role", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    DataRowParser rowParser = new DataRowParser(row);
                    roles.Add(rowParser.ParseObject<Role>());
                }
            }

        }

        public void CreateRole(string name)
        {
            Role role = new Role(name);
            roles.Add(role);

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("INSERT INTO dbo.Role (Id, Name) VALUES (@id, @name)", conn);
                SqlParameter idParam = new SqlParameter("@id", SqlDbType.Char, 36);
                idParam.Value = role.Id.ToString();
                SqlParameter nameParam = new SqlParameter("@name", SqlDbType.VarChar, 255);
                nameParam.Value = role.Name;
                command.Parameters.Add(idParam);
                command.Parameters.Add(nameParam);

                command.Prepare();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteRole(Role role)
        {
            roles.Remove(role);

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("DELETE FROM dbo.Role WHERE id = @id", conn);
                SqlParameter idParam = new SqlParameter("@id", SqlDbType.Char, 36);
                idParam.Value = role.Id.ToString();
                command.Parameters.Add(idParam);

                command.Prepare();
                command.ExecuteNonQuery();
            }
        }
    }
}
