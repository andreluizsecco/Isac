using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Isac.Bot.Services
{
    public class Messages
    {
        public static async Task Save(string message)
        {
            var url = "appname.azurewebsites.net";
            var token = "YourToken";
            var client = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"http://{url}/api/Messages/AddMessage"));
            requestMessage.Content = new StringContent($"token={token}&message={message}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await client.SendAsync(requestMessage);
        }
    }
}