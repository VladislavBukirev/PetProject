using System.Net.Mime;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
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
            const string FirstButton = "FirstButton";
            const string SecondButton = "SecondButton";
            const string ThirdButton = "ThirdButton";
            const string FourthButton = "FourthButton";
            
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
                            var updates = bot.GetUpdatesAsync(offset).Result; //Регистрируем изменение
                            if (updates != null && updates.Length > 0)
                            {
                                foreach (var update in updates)
                                {
                                    ProcessUpdate(update); //Обновляем состояние бота
                                    offset = update.Id + 1; //Изменяем offset изменения, чтобы оно не выполнялось вечно
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
                        switch (text)
                        {
                            case FirstButton:
                                bot.SendTextMessageAsync(update.Message.Chat.Id, "Первая кнопка", replyMarkup: GetButtons());
                                break;
                                break;
                            case SecondButton:
                                bot.SendTextMessageAsync(update.Message.Chat.Id, "Вторая кнопка", replyMarkup: GetButtons());
                                break;
                            case ThirdButton:
                                bot.SendTextMessageAsync(update.Message.Chat.Id, "Третья кнопка", replyMarkup: GetButtons());
                                break;
                            case FourthButton:
                                bot.SendTextMessageAsync(update.Message.Chat.Id, "Четвёртая кнопка", replyMarkup: GetButtons());
                                break;
                            default:
                                bot.SendTextMessageAsync(update.Message.Chat.Id, "Recieved text: " + text, replyMarkup: GetButtons());
                                break;
                        }
                        break;
                    }
                    default:
                        Console.WriteLine(update.Type + "Not Implemented");
                        break;
                }
            }

            private IReplyMarkup GetButtons()
            {
                return new ReplyKeyboardMarkup(token) //Передаём в клавиатуру токен нашего бота
                {
                Keyboard = new List<IEnumerable<KeyboardButton>>
                    {
                        new List<KeyboardButton> //Первая строка кнопок
                        {
                            new KeyboardButton(FirstButton),
                            new KeyboardButton(SecondButton)
                        },
                        new List<KeyboardButton> //Вторая строка кнопок
                        {
                            new KeyboardButton(ThirdButton),
                            new KeyboardButton(FourthButton)
                        }
                    }
                };
            }
        }
    }
}