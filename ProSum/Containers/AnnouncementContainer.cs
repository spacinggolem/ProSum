using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProSum.Containers
{
    public class AnnouncementContainer : IAnnouncementContainer
    {
        private readonly string ConnectionString;

        public AnnouncementContainer(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<Announcement> Get(Guid projectId)
        {
            List<Announcement> result = new List<Announcement>();

            string query = $"SELECT * " +
                $"FROM dbo.Announcement " +
                $"WHERE ProjectId = @projectId";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@projectId", projectId.ToString());

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    Announcement announcement = parser.ParseObject<Announcement>();
                    result.Add(announcement);
                }



                return result;
            }
        }

        public void AddAnnouncementToDB(Announcement announcement)
        {
            string query = $"INSERT INTO dbo.Announcement (Id, ProjectId, AuthorId, Title, Message) " +
                $"VALUES (@Id, @projectId, @authorId, @title, @message)";

            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", announcement.AnnouncementId.ToString());
                command.Parameters.AddWithValue("@projectId", announcement.ProjectId.ToString());
                command.Parameters.AddWithValue("@authorId", announcement.AuthorId.ToString());
                command.Parameters.AddWithValue("@title", announcement.Title);
                command.Parameters.AddWithValue("@message", announcement.Message);

                command.ExecuteNonQuery();
                
            }
        }
    }
}
