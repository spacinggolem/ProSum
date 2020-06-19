using Microsoft.Data.SqlClient;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Models.DataBase;
using ProSum.Services.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace ProSum.Containers
{
    public class StepContainer : IStepContainer
    {
        private List<Step> steps;
        public IReadOnlyList<Step> Steps 
        { 
            get 
            {
                return steps;
            } 
        }
        private string ConnectionString;

        public StepContainer(string connectionString)
        {
            ConnectionString = connectionString;
            steps = new List<Step>();
            LoadAllStoredSteps();
        }

        private void LoadAllStoredSteps()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = $"SELECT * FROM dbo.Step";
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dt = new DataTable();
                sqlDataAdapter.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    DataRowParser rowParser = new DataRowParser(row);
                    steps.Add(rowParser.ParseObject<Step>());
                }
            }
        }

        public void CreateStep(Step step)
        {
            using(SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "INSERT INTO dbo.Step (Id, Name, StepNumber) " +
                    "VALUES (@Id, @Name, @StepNumber)";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", step.Id);
                command.Parameters.AddWithValue("@Name", step.Name);
                command.Parameters.AddWithValue("@StepNumber", step.StepNumber);

                command.ExecuteNonQuery();
                steps.Add(step);
            }
        }

        public void UpdateStepNumber(Step step)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "UPDATE dbo.Step " +
                    "SET StepNumber = @StepNumber " +
                    "WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", step.Id);
                command.Parameters.AddWithValue("@StepNumber", step.StepNumber);

                command.ExecuteNonQuery();
                // Remove current step in list
                steps.Remove(steps.Find(Step => Step.Id == step.Id));
                // Add updated version
                steps.Add(step);
            }
        }

        public void DeleteStep(Step step)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "DELETE FROM dbo.Step " +
                    "WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", step.Id);

                command.ExecuteNonQuery();
                // Remove current step in list
                steps.Remove(steps.Find(Step => Step.Id == step.Id));
            }
        }

        public void UpdateStepName(Step step)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "UPDATE dbo.Step " +
                    "SET Name = @Name " +
                    "WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@Id", step.Id);
                command.Parameters.AddWithValue("@Name", step.Name);

                command.ExecuteNonQuery();
                // Remove current step in list
                steps.Remove(steps.Find(Step => Step.Id == step.Id));
                // Add updated version
                steps.Add(step);
            }
        }
    }
}
