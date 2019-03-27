using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using AdaptiveCards;

namespace QnABot.Dialogs
{
    [Serializable]
    public class RatingDialog:IDialog<object>
    {


        public async Task StartAsync(IDialogContext context)
        {


            //  PromptDialog.Choice(context, MenuChoiceReceivedasync, new List<string>() { "★★★★★", "★★★★", "★★★", "★★", "★" }, " ", " ", 1);

            Activity replyToConversation = (Activity)context.MakeMessage();
            replyToConversation.Attachments = new List<Attachment>();
            AdaptiveCard card = new AdaptiveCard();

            card.Body.Add(new TextBlock()
            {
                Text = "Rate the AuditBot",
                Size = TextSize.Large,
                Weight = TextWeight.Bolder
            });

            //Action
            card.Actions.Add(new SubmitAction()
            {
                Title= "★★★★★",
                Data= "You had rate ★★★★★ "
            });
            card.Actions.Add(new SubmitAction()
            {
                Title = "★★★★",
                Data = "You had rate ★★★★"
            });
            card.Actions.Add(new SubmitAction()
            {
                Title = "★★★",
                Data = "You had rate ★★★"
            });
            card.Actions.Add(new SubmitAction()
            {
                Title = "★★",
                Data = "You had rate ★★"
            });
            card.Actions.Add(new SubmitAction()
            {
                Title = "★",
                Data = "You had rate ★"
            });


            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
            replyToConversation.Attachments.Add(attachment);

          
            await context.PostAsync(replyToConversation);
          
            context.Done(true);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Value.Equals("You had rate ★★★★★"))
            {
                //reroute the user back to your card with an additional message to 
                //put response in the provided fields.
                //return;
                await context.PostAsync("Thanks ");
            }

        
            
        }




    }
      

}
