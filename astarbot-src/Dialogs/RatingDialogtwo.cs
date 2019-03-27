using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.QnABot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace QnABot.Dialogs
{
    [Serializable]
    public class RatingDialogtwo : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {

            // Send a feedback Hero Card for the user to select an option
            Activity reply = ((Activity)context.Activity).CreateReply();
            HeroCard card = new HeroCard();
            card.Buttons = new List<CardAction>
                            {
                                new CardAction(ActionTypes.ImBack, "★★★★★", value: "★★★★★"),
                              new CardAction(ActionTypes.ImBack, "★★★★", value: "★★★★"),
                               new CardAction(ActionTypes.ImBack, "★★★", value: "★★★"),
                              new CardAction(ActionTypes.ImBack, "★★", value: "★★"),
                              new CardAction(ActionTypes.ImBack, "★", value: "★"),


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
                case "★★★★★":
                    await context.PostAsync("You had rate ★★★★★");
                    break;
                case "★★★★":
                    await context.PostAsync("You had rate ★★★★");
                    break;
                case "★★★":
                    await context.PostAsync("You had rate ★★★");
                    break;
                case "★★":
                    await context.PostAsync("You had rate ★★");
                    break;
                case "★":
                    await context.PostAsync("You had rate ★");
                    break;

            }

            context.Wait(OnFeedbackSubmitted);
        }
        private async Task OnFeedbackSubmitted(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var feedbackMessage = message.Text;

            context.Done("");
        }
    }
}