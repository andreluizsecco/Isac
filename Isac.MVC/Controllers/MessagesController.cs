using Isac.MVC.DAL;
using Isac.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Isac.MVC.Controllers
{
    public class MessagesController : Controller
    {
        [HttpPost]
        [Route("api/Messages/AddMessage")]
        public async Task AddMessage(Guid token, string message)
        {
            if (Guid.Parse("YourToken").Equals(token))
            {
                var textAnalyzer = new TextAnalyzer();
                var language = await textAnalyzer.GetMessageLanguageAsync(message);
                var score = await textAnalyzer.GetSentimentScoreAsync(language, message);
                Commands.AddMessage(message, language, score);
            }
        }
    }
}