using System;
using System.Data.SqlClient;

namespace Isac.Bot.Services
{
    public class DHT11
    {
        static string connectionString = $"YourConnectionString";

        public static string GetInfByName(string inf)
        {
            string result = string.Empty;
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TOP(1) Temperature, Humidity FROM dbo.DHT11 ORDER BY Date DESC", conn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            switch (inf)
                            {
                                case "temperature":
                                    result = reader["Temperature"].ToString() + " ºC";
                                    break;
                                case "humidity":
                                    result = reader["Humidity"].ToString() + " %";
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return result;
        }
    }
}