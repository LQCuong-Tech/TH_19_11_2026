using System.Data.SqlClient;

namespace Bai_6_5_KaraokeManagement.DAL
{
    public class DbConnection
    {
        private static readonly string ConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=KaraokeDB;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}