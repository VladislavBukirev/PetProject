using Telegram.Bot;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExperiments
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TelegramBotHelper hlp = new TelegramBotHelper(token: "6271910487:AAH2yNNXNTPtVr5azFv_HcGl6haBZuAiuws");
                hlp.GetUpdates();
            }
            catch (Exception e) {Console.WriteLine(e);}
        }

        private class TelegramBotHelper
        {
            private string token;
            private TelegramBotClient bot;

            public TelegramBotHelper(string token)
            {
                this.token = token;
            }

            public void GetUpdates()
            {
                bot = new TelegramBotClient(token);
                var me = bot.GetMeAsync().Result;
                if (me != null && !string.IsNullOrEmpty(me.Username))
                {
                    int offset = 0;
                    while (true)
                    {
                        try
                        {
                            var updates = bot.GetUpdatesAsync(offset).Result;
                            if (updates != null && updates.Length > 0)
                            {
                                foreach (var update in updates)
                                {
                                    ProcessUpdate(update);
                                    offset = update.Id + 1;
                                }
                            }
                        }
                        catch (Exception e) {Console.WriteLine(e);}
                        
                        Thread.Sleep(1000);
                    }
                }
            }

            private void ProcessUpdate(Telegram.Bot.Types.Update update)
            {
                switch (update.Type)
                {
                    case Telegram.Bot.Types.Enums.UpdateType.Message:
                    {
                        var text = update.Message.Text;
                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Recieved text: " + text);
                        break;
                    }
                    default:
                        Console.WriteLine(update.Type + "Not Implemented");
                        break;
                }
            }
        }
    }
}