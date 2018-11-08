using System;
using System.Data;
using System.Data.SqlClient;

namespace Isac.MVC.DAL
{
    public class SqlBase
    {
        private static readonly string _connectionString = $"YourConnectionString";

        protected delegate void SqlReader(SqlDataReader reader);

        protected static void ExecuteQuery(string sql, SqlReader sqlReader)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sqlReader(reader);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex) { }
        }

        protected static void ExecuteNonQuery(string sql, CommandType type, SqlParameter[] parameters)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        cmd.CommandType = type;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex) { }
        }
    }
}
