using Isac.MVC.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Isac.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() =>
            View();

        public IActionResult Sensors() =>
            View();

        public IActionResult ChatAnalyzer()
        {
            ViewBag.Score = Queries.GetSentimentScoreAverage();
            ViewBag.Languages = Queries.GetLanguages();

            var emoticon = "0.png";
            if (!string.IsNullOrEmpty(ViewBag.Score))
            {
                var score = float.Parse(ViewBag.Score);

                if (score < 25)
                    emoticon = "0.png";
                else if (score < 50)
                    emoticon = "25.png";
                else if (score < 75)
                    emoticon = "50.png";
                else if (score < 100)
                    emoticon = "75.png";
                else
                    emoticon = "100.png";

                ViewBag.Emoticon = emoticon;
                ViewBag.Color = score > 50 ? "Green" : "Red";

            }
            return View();
        }

        public IActionResult ClearMessages()
        {
            Commands.ClearMessages();
            return RedirectToAction("ChatAnalyzer");
        }
    }
}
