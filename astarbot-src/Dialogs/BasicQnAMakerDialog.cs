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

        private const string relevantAnswer = "Answer is Relevant";
        private const string NotrelevantAnswer = "No Relevant Answer";
        private const string RephraseQuestion = "Rephrase My Question";
        private const string ExternalConsultant = "Seek for External Advice";
        private const string Quit = "Quit";

        //async
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
           
           // context.Wait(MessageReceivedAsync);
     
            this.ShowOptions(context);
           // await context.Forward(new AnswerDialog());


        }


        private void ShowOptions(IDialogContext context)
        {

              PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { relevantAnswer, NotrelevantAnswer }, " ", "Not a valid option", 3);
       

        }


        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case relevantAnswer:
                      //  context.Call(new RatingDialog(), this.ResumeAfterOptionDialog);
                       
                        await context.PostAsync("Your Feedback Been Recorded.");
                        this.ShowOptions3(context);
                        break;
                    case NotrelevantAnswer:

                         //this.ShowRating(context);
                       // await this.ShowRating(context);
                        context.Call(new RatingDialog(), this.ResumeAfterOptionDialog);
                       // context.Done(true);
                        break;
                        
                }
            }

            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
           

            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                context.Done(true);
                //await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

      
          
       



        private void ShowOptions3(IDialogContext context)
        {

            PromptDialog.Choice(context, this.OnOptionSelected3, new List<string>() { RephraseQuestion, ExternalConsultant, Quit }, " ", "Not a valid option", 3);

        }
        private async Task OnOptionSelected3(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case RephraseQuestion:
                        context.Wait(this.MessageReceivedAsync);
                        break;
                    case ExternalConsultant:
                        await context.PostAsync("Thanks for contacting our support team");
                        break;
                    case Quit:
                        context.Call(new RatingDialog(), this.ResumeAfterOptionDialog);
                       // context.Done(true);
                        break;

                }
            }

            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
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
        public BasicQnAMakerPreviewDialog() : base(new QnAMakerService(new QnAMakerAttribute(qnaAuthKey, qnaKBId, "I do not know answer to your question how else can i help you?\n\nPlease ask me questions again.", 0.5, 3, endpointHostName: endpointHostName)))
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
                 "I do not know answer to your question how else can i help you?\n\n" +
                 "Please ask me questions again.",
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

            //if(firstanswer== "I do not know answer to your question how else can i help you?\n\nPlease ask me questions again.")
            //{

            //}

            Activity answer = ((Activity)context.Activity).CreateReply();


            var dataAnswer = firstanswer.Split(';');

            if (dataAnswer.Length == 1)
            {

                await context.PostAsync(firstanswer);
                return;
            }
            //else
            //{
            //     AdaptiveCard adaptiveCard = new AdaptiveCard();

            //     //Set the fallback text in case someone sends a request from a client that doesn't yet support Adaptive Cards fully
            //     adaptiveCard.FallbackText = firstanswer;

            //     // Add text to the card.
            //     adaptiveCard.Body.Add(new TextBlock()
            //     {
            //        Text = firstanswer,
            //        Wrap = true,

            //    });

            //     // Add text to the card.
            //     adaptiveCard.Body.Add(new TextBlock()
            //     {
            //         Text = "Was this answer helpful?",
            //         Size = TextSize.Small
            //     });

            //     // Add buttons to the card.
            //     adaptiveCard.Actions.Add(new SubmitAction()
            //     {
            //         Title = "Yes",
            //         Data = "Yes, this was helpful"
            //     });

            //     adaptiveCard.Actions.Add(new SubmitAction()
            //     {
            //         Title = "No",
            //         Data = "No, this was not helpful"
            //     });

            //     // Create the attachment.
            //     Attachment attachment = new Attachment()
            //     {
            //         ContentType = AdaptiveCard.ContentType,
            //         Content = adaptiveCard
            //     };

            //     answer.Attachments.Add(attachment);
            //}


            var title = dataAnswer[0];
            var description = dataAnswer[1];
            var url = dataAnswer[2];
            var imageUrl = dataAnswer[3];

            HeroCard card = new HeroCard
            {

                Text = description

            };

            ////    card.Buttons = new List<CardAction>
            ////{
            ////    new CardAction(ActionTypes.OpenUrl,"Learn More", value:url)
            ////};

            ////    card.Images = new List<CardImage>
            ////{
            ////    new CardImage(url = imageUrl)
            ////};

            answer.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            answer.Attachments.Add(card.ToAttachment());

            await context.PostAsync(answer);
        }



    }
}