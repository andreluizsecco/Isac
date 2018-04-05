using Tweetinvi;

namespace Isac.Bot.Services
{
    public class Twitter
    {
        const string _consumerKey = "YourConsumerKey";
        const string _consumerSecret = "YourConsumerSecret";
        const string _userAccessToken = "YourUserAccessToken";
        const string _userAccessSecret = "YourUserAccessSecret";

        public Twitter() =>
            Auth.SetUserCredentials(_consumerKey, _consumerSecret, _userAccessToken, _userAccessSecret);

        public void PublishTweet(string msg) =>
            Tweet.PublishTweet(msg);
    }
}