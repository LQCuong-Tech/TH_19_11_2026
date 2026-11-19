using System.Data.SqlClient;

namespace Bai_6_3_ProjectTaskManagement.DAL
{
    public class DbConnection
    {
        private static readonly string ConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=ProjectTaskDB;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}