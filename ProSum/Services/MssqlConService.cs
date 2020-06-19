using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProSum.Services.Interfaces;

namespace ProSum.Services
{
    public class MssqlConService : IMssqlConService
    {
        public MssqlConService(IConfiguration configuration, string connectionStringName)
        {
            connection = new SqlConnection(configuration.GetConnectionString(connectionStringName));
        }

        private SqlConnection connection;
        public SqlConnection Connection => connection;

        public bool Connect()
        {
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            return connection.State == System.Data.ConnectionState.Open;
        }

        public bool Disconnect()
        {
            if(connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
            return connection.State == System.Data.ConnectionState.Closed;
        }
    }
}
