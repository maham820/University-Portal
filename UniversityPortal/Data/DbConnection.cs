using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace UniversityPortal.Data
{
    public static class DbConnection
    {
        // Database connection string
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        // Create and return a new SQL connection
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // SELECT queries (returns DataTable)
        public static DataTable GetData(
            string query,
            params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        // INSERT / UPDATE / DELETE queries
        public static int ExecuteCommand(
            string query,
            params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        // Queries that return a single value
        public static object GetSingleValue(
            string query,
            params SqlParameter[] parameters)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteScalar();
            }
        }

        // Read data using SqlDataReader
        // Caller must close the connection
        public static SqlDataReader GetReader(
            string query,
            SqlConnection connection,
            params SqlParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(query, connection);

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }
}
