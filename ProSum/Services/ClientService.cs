using Microsoft.Data.SqlClient;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProSum.Services
{
    public class ClientService : IClientService
    {
        private string ConnectionString;

        public ClientService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Create(Client client)
        {
            string query = $"INSERT INTO dbo.Client (Id, Name, Email, Company, PhoneNumber) " +
                $"VALUES ('{client.Id}', '{client.Name}', '{client.Email}', '{client.Company}', '{client.PhoneNumber}')";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand q = new SqlCommand(query, conn);
                q.ExecuteNonQuery();

            }
        }
        public Client GetByEmail(string email)
        {
            Client client = null;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("select * from dbo.Client WHERE email = @Email", conn);
                command.Parameters.AddWithValue("@Email", email);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count > 0)
                {
                    DataRowParser parser = new DataRowParser(table.Rows[0]);
                    client = parser.ParseObject<Client>();
                }


            }

            return client;
        }

        public List<Client> Get()
        {
            string query = $"SELECT * " +
                $"FROM Client ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                List<Client> result = new List<Client>();

                foreach (DataRow row in table.Rows)
                {
                    DataRowParser parser = new DataRowParser(row);
                    Client client = parser.ParseObject<Client>();
                    result.Add(client);
                }

                return result;
            }
        }

        public Client Get(Guid id)
        {
            string query = $"SELECT * " +
                $"FROM Client " +
                $"WHERE Id = '{id}'";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(query, conn);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);

                Client client = new Client();

                if (table.Rows[0] != null)
                {
                    DataRowParser parser = new DataRowParser(table.Rows[0]);
                    client = parser.ParseObject<Client>();
                }
                else
                {
                    client = null;
                }

                return client;
            }
        }

        public void Edit(Guid guidToUpdate, Client client)
        {
            string query = $"UPDATE Client " +
                $"SET Name='{client.Name}', Email='{client.Email}', Company='{client.Company}', PhoneNumber='{client.PhoneNumber}'" +
                $"WHERE Id='{guidToUpdate}'";


            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand q = new SqlCommand(query, conn);
                q.ExecuteNonQuery();
            }
        }
    }
}
