using Microsoft.Data.SqlClient;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProSum.Containers
{
    public class PermissionContainer : IPermissionContainer
    {
        private List<Permission> permissions;
        public IReadOnlyList<Permission> Permissions { 
            get
            {
                return permissions;
            }
        }
        private string ConnectionString;

        public PermissionContainer(string connectionString)
        {
            permissions = new List<Permission>();
            ConnectionString = connectionString;
            GetPermissionStored();
        }

        private void GetPermissionStored()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.Permission", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                foreach(DataRow row in dt.Rows)
                {
                    DataRowParser rowParser = new DataRowParser(row);
                    permissions.Add(rowParser.ParseObject<Permission>());
                }
            }
        }

        public void CreatePermission(string name)
        {
            Permission perm = new Permission(Guid.NewGuid(), name);
            permissions.Add(perm);
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("INSERT INTO dbo.Permission (Id, Name) VALUES (@id, @name)", conn);
                SqlParameter idParam = new SqlParameter("@id", SqlDbType.Char, 36);
                idParam.Value = perm.Id.ToString();
                SqlParameter nameParam = new SqlParameter("@name", SqlDbType.VarChar, 255);
                nameParam.Value = perm.Name;
                command.Parameters.Add(idParam);
                command.Parameters.Add(nameParam);

                command.Prepare();
                command.ExecuteNonQuery();
            }
        }

        public void UpdatePermissionName(string newName, string oldName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand commandWritePermission = new SqlCommand("UPDATE dbo.Permission " +
                    "SET Name=@newReadName " +
                    "WHERE Name = @oldReadName", conn);

                commandWritePermission.Parameters.AddWithValue("@newReadName", "Write"+ newName);
                commandWritePermission.Parameters.AddWithValue("@oldReadName", "Write"+ oldName);

                SqlCommand commandReadPermission = new SqlCommand("UPDATE dbo.Permission " +
                    "SET Name=@newWriteName " +
                    "WHERE Name = @oldReadName", conn);

                commandReadPermission.Parameters.AddWithValue("@newWriteName", "Read" + newName);
                commandReadPermission.Parameters.AddWithValue("@oldReadName", "Read" + oldName);

                commandWritePermission.ExecuteNonQuery();

                commandReadPermission.ExecuteNonQuery();
            }
            permissions.Find(Permission => Permission.Name == "Write" + oldName).Name = "Write" + newName;
            permissions.Find(Permission => Permission.Name == "Read" + oldName).Name = "Read" + newName;
        }

        public void DeletePermission(Permission permission)
        {
            permissions.Remove(permission);
        }
    }
}
