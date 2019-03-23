using System;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;

namespace Microsoft.Bot.Sample.QnABot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
       // IDialogContext context;
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity.GetActivityType() == ActivityTypes.Message)
            {

                await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else if (activity.GetActivityType() == ActivityTypes.ConversationUpdate)
            {
                IConversationUpdateActivity iConversationUpdated = activity as IConversationUpdateActivity;
                if (iConversationUpdated != null)
                {
                    ConnectorClient connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));

                    foreach (var member in iConversationUpdated.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                    {
                        // if the bot is added, then 
                        if (member.Id == iConversationUpdated.Recipient.Id)
                        {
                            var reply = ((Activity)iConversationUpdated).CreateReply($"Hi there, I'm AVA! Ask me questions about audit and I'll search my knowledge base for an answer. You can ask me questions like...\n• How is intellectual property defined in IFRS 15 ?\n• Is transaction price equal to contract revenue ? etc");
                            await connector.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
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