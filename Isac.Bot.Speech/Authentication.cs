using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Isac.Bot.Speech
{
    public sealed class Authentication
    {
        // A URL deverá ser alterada, caso não tenha criado seu serviço de speech no Azure na região West US
        public readonly string _fetchTokenUri = "https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private readonly string _subscriptionKey;
        private readonly Timer _accessTokenRenewer;
        private string _token;

        //Access token expires every 10 minutes. Renew it every 9 minutes.
        private const int RefreshTokenDuration = 9;

        public Authentication(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
            _token = FetchToken(_fetchTokenUri, subscriptionKey).Result;

            // renew the token on set duration.
            _accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                            this,
                                            TimeSpan.FromMinutes(RefreshTokenDuration),
                                            TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken() =>
            _token;

        private void RenewAccessToken() =>
            _token = FetchToken(_fetchTokenUri, _subscriptionKey).Result;

        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    _accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        private async Task<string> FetchToken(string fetchUri, string subscriptionKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                UriBuilder uriBuilder = new UriBuilder(fetchUri);

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                Console.WriteLine("Token Uri: {0}", uriBuilder.Uri.AbsoluteUri);
                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}
