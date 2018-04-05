using Isac.MVC.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Isac.MVC.DAL
{
    public class Queries : SqlBase
    {
        public static (string temperature, string humidity) GetSensorsData()
        {
            var temperature = string.Empty;
            var humidity = string.Empty;

            void Reader(SqlDataReader sqlDataReader)
            {
                temperature = sqlDataReader["Temperature"].ToString();
                humidity = sqlDataReader["Humidity"].ToString();
            }

            ExecuteQuery("SELECT TOP(1) Temperature, Humidity FROM dbo.DHT11 ORDER BY Date DESC", Reader);
            return (temperature, humidity);
        }

        public static string GetSentimentScoreAverage()
        {
            string result = string.Empty;

            void Reader(SqlDataReader sqlDataReader)
            {
                result = sqlDataReader["SentimentScore"].ToString();
            }

            ExecuteQuery("SELECT AVG(SentimentScore)*100 as SentimentScore FROM dbo.Messages", Reader);
            return result;
        }

        public static List<ChatLanguageSummary> GetLanguages()
        {
            var result = new List<ChatLanguageSummary>();

            void Reader(SqlDataReader sqlDataReader)
            {
                result.Add(new ChatLanguageSummary()
                {
                    Name = sqlDataReader["Language"].ToString(),
                    Messages = int.Parse(sqlDataReader["Qtd"].ToString())
                });
            }

            ExecuteQuery("SELECT Language, COUNT(ID) Qtd FROM dbo.Messages GROUP BY Language", Reader);
            return result;
        }
    }
}