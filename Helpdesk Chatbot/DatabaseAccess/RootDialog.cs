using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using DatabaseAccess;
using System.Text.RegularExpressions;

namespace Helpdesk_Chatbot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private String tableName = "studenten";
        private String err = "Sorry, I didn't understand you.";
        static SQL sql = new SQL();

        [LuisModel("1913c468-4fe0-423d-8755-677c7c874edc", "734a3eec45aa47d9b2db03f575939c08", LuisApiVersion.V2)]
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        [LuisIntent("SQLRequest")]
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = (await result as Activity).Text;
            var st = "welche studenten wohnen in ";
            var f = "welche studenten studieren ";
            if (Contains(activity, "heißen"))
            {
                var act = Replace(activity, "gib mir alle studenten, die ");
                act = Replace(act, " heißen");
                CreateAnswerAttribute(context, tableName, "Nachname", act);
            }
            else if (Contains(activity, "wohnen"))
            {
                CreateAnswerAttribute(context, tableName, "Stadt", Replace(activity, st));
            }
            else if (Contains(activity, "studieren"))
            {
                CreateAnswerAttribute(context, tableName, "Fach", Replace(activity, f));
            }
            else
            {
                await context.PostAsync(err);
            }
            context.Wait(MessageReceivedAsync);
        }

        private async void CreateAnswerStandard(IDialogContext context, string tableName, string value)
        {
            var cache = sql.executeCommand($"select * from {tableName} where Nachname=\"{value}\"");
            var str = "";
            for (int i = 0; i < cache.Length; i++)
            {
                str += cache[i] + "\n";
            }
            await context.PostAsync(str);
        }

        private async void CreateAnswerAttribute(IDialogContext context, string tableName, string attr, string value)
        {
            var cache = sql.executeCommand($"select * from {tableName} where {attr} =\"{value}\"");
            var str = "";
            for (int i = 0; i < cache.Length; i++)
            {
                str += cache[i] + "\n";
            }
            await context.PostAsync(str);
        }

        private bool Contains(string source, string toCheck)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string Replace(string input, string toReplace)
        {
            return Regex.Replace(input, toReplace, "", RegexOptions.IgnoreCase);
        }
    }
}