using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Connector;
using System.Configuration;
using Microsoft.Bot.Builder.ConnectorEx;
using Newtonsoft.Json;
using System.Linq;
using QnABot.Components;
using QnABot.Models;
using System.Collections.Generic;
using AdaptiveCards;
using System.Collections;
using QnABot.Dialogs;



namespace Microsoft.Bot.Sample.QnABot
{
    [Serializable]

    public class RootDialog : IDialog<object>
    {

        public async Task StartAsync(IDialogContext context)
        {
            /* Wait until the first message is received from the conversation and call MessageReceviedAsync 
            *  to process that message. */

            context.Wait(this.MessageReceivedAsync);

        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */

          // await context.PostAsync("Let me flip through my accounting books and see if I can find anything…");
       
            var message = await result;

            //var qnaAuthKey = GetSetting("QnAAuthKey"); 
            //var qnaKBId = Utils.GetAppSetting("QnAKnowledgebaseId");
            //var endpointHostName = Utils.GetAppSetting("QnAEndpointHostName");

            // Add the code from the line 35 - 37
            var qnaAuthKey = ConfigurationManager.AppSettings["QnAAuthKey"];
            var qnaKBId = ConfigurationManager.AppSettings["QnAKnowledgebaseId"];
            var endpointHostName = ConfigurationManager.AppSettings["QnAEndpointHostName"];

            //// QnA Subscription Key and KnowledgeBase Id null verification
            if (!string.IsNullOrEmpty(qnaAuthKey) && !string.IsNullOrEmpty(qnaKBId))
            {
                // Forward to the appropriate Dialog based on whether the endpoint hostname is present
                if (string.IsNullOrEmpty(endpointHostName))
                    await context.Forward(new BasicQnAMakerPreviewDialog(), AfterAnswerAsync, message, CancellationToken.None);
                else
                    await context.Forward(new BasicQnAMakerDialog(), AfterAnswerAsync, message, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("Please set QnAKnowledgebaseId, QnAAuthKey and QnAEndpointHostName (if applicable) in App Settings. Learn how to get them at https://aka.ms/qnaabssetup.");
            }



        }


        private async Task AfterAnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
           
         //  context.Wait(MessageReceivedAsync);
     
           Activity reply=((Activity)context.Activity).CreateReply();
            HeroCard card = new HeroCard();
            card.Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.ImBack, "Answer is Relevant", value: "Answer is Relevant"),
                                new CardAction(ActionTypes.ImBack, "No Relevant Answer", value: "No Relevant Answer")
            };
            reply.Attachments.Add(card.ToAttachment());
            await context.PostAsync(reply);

            context.Wait(OnOptionSelected2);


        }

        // Route the user to appropriate action depending on their choice
        private async Task OnOptionSelected2(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var choice = message.Text;

            if (choice == "Answer is Relevant" )
            {
                await context.PostAsync("Your feedback has been recorded.");
                await context.PostAsync("Yay! Bot Dance!!");
                await context.PostAsync("└[∵┌] └[ ∵ ]┘ [┐∵]┘");

                context.Call(new FeebackDialog(), OnFeedbackSubmitted);
            }
            else if (choice == "No Relevant Answer")
            {
                await context.PostAsync("Please Rate this Auditbot.");
                context.Call(new RatingDialogtwo(), OnFeedbackSubmitted);
            }
            else
            {
                // Assume it's another question so forward on to QnA dialog
                await context.Forward(child: new BasicQnAMakerDialog(), resume: AfterAnswerAsync, item: message, token: CancellationToken.None);
            }
        }

        private async Task OnFeedbackSubmitted(IDialogContext context, IAwaitable<object> result)
        {
        
            context.EndConversation("");
        }


        public static string GetSetting(string key)
        {
            var value = Utils.GetAppSetting(key);
            if (String.IsNullOrEmpty(value) && key == "QnAAuthKey")
            {
                value = Utils.GetAppSetting("QnASubscriptionKey"); // QnASubscriptionKey for backward compatibility with QnAMaker (Preview)
            }
            return value;
        }
    }

    // Dialog for QnAMaker Preview service
    [Serializable]
    public class BasicQnAMakerPreviewDialog : QnAMakerDialog
    {
        static readonly string qnaAuthKey = ConfigurationManager.AppSettings["QnAAuthKey"];
        static readonly string qnaKBId = ConfigurationManager.AppSettings["QnAKnowledgebaseId"];
        static readonly string endpointHostName = ConfigurationManager.AppSettings["QnAEndpointHostName"];
        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        // Parameters to QnAMakerService are:
        // Required: subscriptionKey, knowledgebaseId, 
        // Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
        public BasicQnAMakerPreviewDialog() : base(new QnAMakerService(new QnAMakerAttribute(qnaAuthKey, qnaKBId, "I don't understand this right now! Try another quetion!", 0.5, 3, endpointHostName: endpointHostName)))
        { }


    }

    // Dialog for QnAMaker GA service
    [Serializable]
    public class BasicQnAMakerDialog : QnAMakerDialog
    {
        static readonly string qnaAuthKey = ConfigurationManager.AppSettings["QnAAuthKey"];
        static readonly string qnaKBId = ConfigurationManager.AppSettings["QnAKnowledgebaseId"];
        static readonly string endpointHostName = ConfigurationManager.AppSettings["QnAEndpointHostName"];

        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        // Parameters to QnAMakerService are:
        // Required: qnaAuthKey, knowledgebaseId, endpointHostName
        // Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
        public BasicQnAMakerDialog() : base(new QnAMakerService(
             new QnAMakerAttribute
             (
                 qnaAuthKey,
                 qnaKBId,
                 "I don't understand this right now! Try another question!",
                 0.5,
                 3,
                 endpointHostName
             )))
        {

        }
 
        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {

            // answer is a string
            var firstanswer = result.Answers.First().Answer;

            Activity answer = ((Activity)context.Activity).CreateReply();
            answer.AttachmentLayout = AttachmentLayoutTypes.Carousel;
          

            answer.Text = firstanswer;

            var dataAnswer = firstanswer.Split(';');



            if (dataAnswer.Length == 1)
            {

                await context.PostAsync(firstanswer);
                return;
            }


            else
           
            {

                var title = dataAnswer[0];
                var description = dataAnswer[1];
                var url = dataAnswer[2];
                var imageUrl = dataAnswer[3];

                HeroCard card = new HeroCard
                {

                    Text = description

                };



                answer.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                answer.Attachments.Add(card.ToAttachment());


            }
        }




    }
}
