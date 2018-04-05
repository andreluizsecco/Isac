using System.Data;
using System.Data.SqlClient;

namespace Isac.MVC.DAL
{
    public class Commands : SqlBase
    {
        public static void AddDht11SensorData(decimal temperature, decimal humidity)
        {
            string command = @"INSERT INTO DHT11(Temperature, Humidity) VALUES (@Temperature, @Humidity)";
            var parameters = new[]
            {
                new SqlParameter("@Temperature", temperature),
                new SqlParameter("@Humidity", humidity)
            };
            ExecuteNonQuery(command, CommandType.Text, parameters);
        }

        public static void AddMessage(string message, string language, double score)
        {
            string command = @"INSERT INTO dbo.Messages (Message, Language, SentimentScore) VALUES (@Message, @Language, @SentimentScore)";
            var parameters = new[]
            {
                new SqlParameter("@Message", message),
                new SqlParameter("@Language", language),
                new SqlParameter("@SentimentScore", score),
            };
            ExecuteNonQuery(command, CommandType.Text, parameters);
        }
    }
}
