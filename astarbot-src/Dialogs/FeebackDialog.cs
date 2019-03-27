using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.QnABot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QnABot.Dialogs
{
    [Serializable]
    public class FeebackDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {

            // Send a feedback Hero Card for the user to select an option
            Activity reply = ((Activity)context.Activity).CreateReply();
            HeroCard card = new HeroCard();
            card.Buttons = new List<CardAction>
                            {
                                new CardAction(ActionTypes.ImBack, "Rephrase my question", value: "Rephrase my question"),
                                new CardAction(ActionTypes.ImBack, "Seek for external advice", value: "Seek for external advice"),
                                new CardAction(ActionTypes.ImBack, "Quit", value: "Quit"),
                              
                            };
            reply.Attachments.Add(card.ToAttachment());
            await context.PostAsync(reply);
            context.Wait(OnOptionSelected);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var choice = message.Text;

            switch (choice)
            {
                case "Rephrase my question":
                    context.Call(new BasicQnAMakerDialog(), OnFeedbackSubmitted);
                    break;
                case "Seek for external advice":
                    await context.PostAsync("Thanks for contacting our support team") ;
                    break;
                case "Quit":
                    //await context.PostAsync("Thank you Dropping By");
                    context.Call(new RatingDialogtwo(), OnFeedbackSubmitted);
                    break;
               
                //default:
                //    await context.PostAsync("Sorry, I didn't understand your choice. Please click one of the buttons.");
                //    context.Wait(OnOptionSelected);
                //    break;
            }

            context.Wait(OnFeedbackSubmitted);
        }
        private async Task OnFeedbackSubmitted(IDialogContext context, IAwaitable<object> result)
        {

            context.EndConversation("");
        }
    }
}