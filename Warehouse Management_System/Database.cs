
using System.Data.SqlClient;

namespace Warehouse_Management_System
{
    public static class Database
    {
        private static string connectionString =
            @"Data Source=GERALDKIMDELOSS\SQLEXPRESS;
              Initial Catalog=WMSystem;
              Integrated Security=True;
              TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}

