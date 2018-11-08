using System;
using System.Linq;
using System.Net;
using Isac.Bot.Speech;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Isac.Bot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Isac.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            // Bing Speech (Speech to text) aceita apenas o formato .WAV
            var audioAttachment = activity.Attachments?.FirstOrDefault(a => a.ContentType.Equals("audio/wav") || a.ContentType.Equals("audio/ogg"));
            if (audioAttachment != null)
            {
                Task.Run(() =>
                {
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var stream = new Audio().GetAudioStream(connector, audioAttachment).Result;
                    activity.Text = new SpeechService().GetTextFromAudioAsync(stream).Result;
                    SendMessage(activity);
                });
            }
            else if (activity.Type == ActivityTypes.Message)
                SendMessage(activity);
            else
                HandleSystemMessage(activity);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async void SendMessage(Activity activity)
        {
            Task.Run(() =>
            {
                Messages.Save(activity.Text);
                Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            });
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}