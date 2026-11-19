using System.Data.SqlClient;

namespace Bai_6_4_GarageManagement.DAL
{
    public class DbConnection
    {
        private static readonly string ConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=GarageDB;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}