using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace QnABot.Dialogs
{
    [Serializable]
    public class AnswerDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(context, this.AfterSelectOption, new string[] { "Stay in this survey", "Get back to where I was" }, "Hello, you're in the survey dialog. Please pick one of these options");

        }

        private async Task AfterSelectOption(IDialogContext context, IAwaitable<string> result)

        {

            if ((await result) == "Get back to where I was")
            {
                await context.PostAsync("Great, back to the original conversation!");
                context.Done(String.Empty); //Finish this dialog
            }
            else
            {
                await context.PostAsync("I'm still on the survey until you tell me to stop");
                PromptDialog.Choice(context, this.AfterSelectOption, new string[] { "Stay in this survey", "Get back to where I was" }, "Hello, you're in the survey dialog. Please pick one of these options");

            }

        }
    }
}