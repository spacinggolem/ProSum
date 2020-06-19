using Microsoft.Data.SqlClient;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace ProSum.MssqlContext
{
    public class UserService : IUserService
    {
        private readonly IRoleContainer roleContainer;
        private readonly string ConnectionString;


        public UserService(IRoleContainer roleContainer, string connectionString)
        {
            this.roleContainer = roleContainer;
            ConnectionString = connectionString;
        }

        public User Login(User user)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("select * from dbo.Account where Email = @Email AND Softdeleted = 0", conn);
                command.Parameters.AddWithValue("@Email", user.Email);

                DataTable table = new DataTable();
                User retrieved = null;

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);

                    if (table.Rows.Count > 0)
                    {
                        DataRowParser parser = new DataRowParser(table.Rows[0]);
                        retrieved = parser.ParseObject<User>();

                        retrieved.Role = roleContainer.Roles.ToList().Find(r => r.Id == Guid.Parse(parser.GetField<string>("RoleId")));
                    }
                    return retrieved;
                }
            }
        }

        public List<User> GetAll()
        {
            List<User> users = new List<User>();

            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("select * from dbo.Account WHERE Softdeleted = 0", conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    User user = parser.ParseObject<User>();

                    user.Role = roleContainer.Roles.FirstOrDefault(r => r.Id == Guid.Parse(parser.GetField<string>("RoleId")));
                    users.Add(user);
                }
            }

            return users;
        }

        public User Get(Guid id)
        {
            User user = null;

            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("select * from dbo.Account WHERE id = @AccountId", conn);
                command.Parameters.AddWithValue("@AccountId", id.ToString());

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    DataRowParser parser = new DataRowParser(table.Rows[0]);
                    user = parser.ParseObject<User>();
                    user.Role = roleContainer.Roles.ToList().Find(r => r.Id == Guid.Parse(parser.GetField<string>("RoleId")));
                }

                
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            User user = null;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("select * from dbo.Account WHERE email = @AccountEmail AND Softdeleted = 0", conn);
                command.Parameters.AddWithValue("@AccountEmail", email);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    DataRowParser parser = new DataRowParser(table.Rows[0]);
                    user = parser.ParseObject<User>();
                }
            }
            return user;
        }

        public List<User> GetAllWithRole(RolesEnum role)
        {
            List<User> users = new List<User>();
            Guid roleId = roleContainer.Roles.FirstOrDefault(r => r.Name == role.ToString()).Id;
            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("select * from dbo.Account WHERE RoleId = @RoleId AND Softdeleted = 0", conn);
                command.Parameters.AddWithValue("@RoleId", roleId.ToString());

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
        public void Update(User user)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {

                string query = $"UPDATE Account SET " + 
                    $"FirstName='{user.FirstName}', " +
                    $"LastName='{user.LastName}', " +
                    $"Email='{user.Email}', " +
                    $"PhoneNumber='{user.PhoneNumber}', " +
                    $"Name='{user.Name}', " +
                    $"Password='{user.Password}'," +
                    $"Department='{user.Department.ToString()}', " +
                    $"RoleId='{user.Role.Id}' " +
                    $"WHERE Id = '{user.Id}'" +
                    $"AND Softdeleted = 0";

                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);
                q.ExecuteNonQuery();

            }
        }

        public void Delete(User user)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {

                string query = $"UPDATE Account SET Softdeleted = 1" +
                    $"WHERE Id = '{user.Id}'";

                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);
                q.ExecuteNonQuery();

            }
        }
    }
}
