using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Isac.MVC.Services
{
    public class TextAnalyzer
    {
        private const string SubscriptionKey = "YourKey";

        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }

        private readonly ITextAnalyticsClient client;

        public TextAnalyzer()
        {

            client = new TextAnalyticsClient(new ApiKeyServiceClientCredentials())
            {
                Endpoint = "https://eastus2.api.cognitive.microsoft.com"
            };
        }

        public async Task<string> GetMessageLanguageAsync(string message)
        {
            var language = string.Empty;

            LanguageBatchResult result = await client.DetectLanguageAsync(
                    new BatchInput(
                        new List<Input>()
                        {
                            new Input("1", message)
                        }
            ));

            if (result.Documents.Count > 0)
                language = result.Documents[0].DetectedLanguages[0].Iso6391Name;

            return language;
        }


        public async Task<double> GetSentimentScoreAsync(string language, string message)
        {
            double score = -1;

            SentimentBatchResult result = await client.SentimentAsync(
                    new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput(language, "1", message)
                        }));

            if (result.Documents.Count > 0)
                score = result.Documents[0].Score.Value;

            return score;
        }
    }
}
