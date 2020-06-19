using Microsoft.Data.SqlClient;

namespace ProSum.Services.Interfaces
{
    public interface IMssqlConService
    {
        public SqlConnection Connection { get; }
        public bool Connect();
        public bool Disconnect();
    }
}
