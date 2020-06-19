using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProSum.Models;
using ProSum.Services.Interfaces;

namespace ProSum.MssqlContext
{
    public class AdminService : IAdminService
    {
        private readonly string ConnectionString;
        public AdminService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void CreateUser(User newuser)
        {
        string query = $"INSERT INTO dbo.Account (Id, RoleId, Name, FirstName, LastName, Email, Password, PhoneNumber, Department) " +
                $"VALUES ('{newuser.Id}', '{newuser.Role.Id}', '{newuser.Name}', '{newuser.FirstName}', '{newuser.LastName}',  '{newuser.Email}', '{newuser.Password}', '{newuser.PhoneNumber}', '{newuser.Department}')";
            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);
                q.ExecuteNonQuery();
                
            }
        }
    }
}
