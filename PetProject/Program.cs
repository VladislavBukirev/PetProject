using System.Net.Mime;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static TelegramBotExperiments.Images;
using File = Telegram.Bot.Types.File;

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
            const string GetImgButton = "Хочу картинку";
            const string AddHomeworkButton = "Добавить домашнее задание";

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
                        string image = null;
                        switch (text)
                        {
                            case GetImgButton:
                                var rng = new Random();
                                image = Path.Combine(Environment.CurrentDirectory, Images.GetImage(rng.Next(3))); //достать картинку
                                using (var stream = System.IO.File.OpenRead(image)) //открыть картинку
                                {
                                    var r = bot.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), replyMarkup: GetButtons()).Result;
                                }
                                break;
                            case AddHomeworkButton:
                                image = Path.Combine(Environment.CurrentDirectory, "101.png"); //достать картинку
                                using (var stream = System.IO.File.OpenRead(image)) //открыть картинку
                                {
                                    var r = bot.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), replyMarkup: GetInlineButtons("1")).Result;
                                }
                                break;
                            default:
                                bot.SendTextMessageAsync(update.Message.Chat.Id, "Recieved text: " + text, replyMarkup: GetButtons());
                                break;
                        }
                        break;
                    }
                    case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                        switch (update.CallbackQuery.Data)
                        {
                            case "1":
                                var msg = bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Загружай", replyMarkup: GetButtons()).Result;
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine(update.Type + "Not Implemented");
                        break;
                }
            }

            private IReplyMarkup GetInlineButtons(string id)
            {
                return new InlineKeyboardMarkup(new InlineKeyboardButton(id)
                    { Text = "Добавить", CallbackData = id.ToString() });
            }

            private IReplyMarkup GetButtons()
            {
                return new ReplyKeyboardMarkup(token) //Передаём в клавиатуру токен нашего бота
                {
                Keyboard = new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> //Первая строка кнопок
                        {
                            new KeyboardButton(GetImgButton),
                            new KeyboardButton(AddHomeworkButton)
                        },
                    },
                ResizeKeyboard = true
                };
            }
        }
    }
}