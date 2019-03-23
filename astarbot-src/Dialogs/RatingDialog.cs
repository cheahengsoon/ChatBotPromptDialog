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
namespace QnABot.Dialogs
{
    [Serializable]
    public class RatingDialog:IDialog<object>
    {
        //Rating
        private const string fiverate = "★★★★★";
        private const string fourrate = "★★★★";
        private const string threerate = "★★★";
        private const string tworate = "★★";
        private const string Onerate = "★";

        public async Task StartAsync(IDialogContext context)
        {
            // PromptDialog.Choice(context, this.AfterSelectOption, new string[] { "★★★★★", "★★★★" }, "Hello, you're in the survey dialog. Please pick one of these options");
          // PromptDialog.Choice(context, this.AfterSelectOption, new string[] { "★★★★★", "★★★★" }, " ", "Not a valid option", 3, PromptStyle.Keyboard);
           PromptDialog.Choice(context, this.OnOptionRating, new List<string>() { fiverate, fourrate, threerate, tworate, Onerate }, " ", " ", 1, PromptStyle.Keyboard);
            // context.Done(String.Empty);
            //context.Wait(this.MessageReceivedAsync);
            //this.ShowOptions(context);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;


            //this.ShowOptions(context);

        }
        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionRating, new List<string>() { fiverate, fourrate }, "rating", "Not a valid option", 3);
        }

        private async Task OnOptionRating(IDialogContext context, IAwaitable<string> result)
        {

            try
            {
                var optionSelected = await result;
                var message = context.MakeMessage();
                 await context.PostAsync($"OptionSelected {message}.");

                switch (optionSelected)
                {
                    case fiverate:
                        await context.PostAsync("Five Star");
                        context.Done(true);
                        break;
                    case fourrate:
                        await context.PostAsync("Four Star");
                        context.Done(true);
                        break;
                    case threerate:
                       await context.PostAsync("Three Star");
                       // context.Done(true);
                        break;
                    case tworate:
                        await context.PostAsync("Two Star");
                       // context.Done(true);
                        break;
                    case Onerate:
                        await context.PostAsync("One Star");
                        //context.Done(true);
                        break;

                }
            }

            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

               context.Wait(this.MessageReceivedAsync);
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }


        private async Task AfterSelectOption(IDialogContext context, IAwaitable<string> result)

        {

            if ((await result).ToString() == "★★★★★")
            {
                string value = "1";
                await context.PostAsync(value);
                //context.Done(String.Empty); //Finish this dialog
            }
            else
            {
                await context.PostAsync("I'm still on the survey until you tell me to stop");
               // PromptDialog.Choice(context, this.AfterSelectOption, new string[] { "Stay in this survey", "Get back to where I was" }, "Hello, you're in the survey dialog. Please pick one of these options");

            }

        }



    }
      

}
