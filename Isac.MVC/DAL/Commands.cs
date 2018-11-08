using System.Data;
using System.Data.SqlClient;

namespace Isac.MVC.DAL
{
    public class Commands : SqlBase
    {
        public static void AddDht11SensorData(string sender, decimal temperature, decimal humidity)
        {
            string command = @"INSERT INTO DHT11(Sender, Temperature, Humidity) VALUES (@Sender, @Temperature, @Humidity)";
            var parameters = new[]
            {
                new SqlParameter("@Sender", sender),
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

        public static void ClearMessages()
        {
            string command = @"DELETE FROM dbo.Messages";
            ExecuteNonQuery(command, CommandType.Text, null);
        }
    }
}
