using System.Data;
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
            private Telegram.Bot.TelegramBotClient bot;

            const string GetImgButton = "Хочу картинку";
            const string AddHomeworkButton = "Добавить домашнее задание";
            const string HomeworkStatusButton = "Узнать состояние домашнего задания";
            //Словарь содержит группу и другой словарь, содержащий дату и домашние задания, заданные на эту дату
            private Dictionary<string, Dictionary<string, List<string>>> HomeworkDict = new Dictionary<string, Dictionary<string, List<string>>>();

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
                        {
                            string image;
                            switch (text)
                            {
                                case GetImgButton:
                                    var rnd = new Random();
                                    image = Path.Combine(Environment.CurrentDirectory, Images.GetImage(rnd.Next(3))); //достать картинку
                                    using (var stream = System.IO.File.OpenRead(image)) //открыть картинку
                                    {
                                        var r = bot.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), replyMarkup: GetMainMenuButtons()).Result;
                                    }
                                    break;
                                case AddHomeworkButton:
                                    image = Path.Combine(Environment.CurrentDirectory, "101.png"); //достать картинку
                                    bot.SendTextMessageAsync(update.Message.Chat.Id, "Выберите вашу группу", replyMarkup: GetGroupButtons());
                                    var day = DateTime.Today;
                                    var list = new List<string>();
                                    var a = text;
                                    list.Add(a);
                                    var dict = new Dictionary<string, List<string>>();
                                    dict.Add(day.ToString(), list);
                                    HomeworkDict.Add("ФТ-103-2", dict);
                                    using (var stream = System.IO.File.OpenRead(image)) //открыть картинку
                                    {
                                        var r = bot.SendPhotoAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), replyMarkup: GetInlineButtons("1")).Result;
                                    }
                                    break;
                                case HomeworkStatusButton:
                                    var dayy = DateTime.Today;
                                    bot.SendTextMessageAsync(update.Message.Chat.Id, HomeworkDict["ФТ-103-2"][dayy.ToString()][0],
                                        replyMarkup: GetGroupButtons());
                                    break;
                                // case "ФТ-103-1":
                                //     bot.SendTextMessageAsync(update.Message.Chat.Id, "Вы учитесь в ФТ-103-1");
                                //     break;
                                // case "ФТ-103-2":
                                //     bot.SendTextMessageAsync(update.Message.Chat.Id, "Вы учитесь в ФТ-103-2");
                                //     break;
                                default:
                                    bot.SendTextMessageAsync(update.Message.Chat.Id, "Recieved text: " + text, replyMarkup: GetMainMenuButtons());
                                    break;
                            }
                        }
                        break;
                    }
                    case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                        switch (update.CallbackQuery.Data)
                        {
                            case "1":
                                var msg = bot.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Загружай", replyMarkup: GetMainMenuButtons()).Result;
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine(update.Type + "Not Implemented");
                        break;
                }
            }
            private IReplyMarkup GetGroupButtons()
            {
                return new ReplyKeyboardMarkup(token) //Передаём в клавиатуру токен нашего бота
                {
                    Keyboard = new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> 
                        {
                            new KeyboardButton("ФТ-101-1"),
                            new KeyboardButton("ФТ-101-2")
                        },
                        new List<KeyboardButton> 
                        {
                            new KeyboardButton("ФТ-102-1"),
                            new KeyboardButton("ФТ-102-2")
                        },
                        new List<KeyboardButton> 
                        {
                            new KeyboardButton("ФТ-103-1"),
                            new KeyboardButton("ФТ-103-2")
                        },
                        new List<KeyboardButton> 
                        {
                            new KeyboardButton("ФТ-104-1"),
                            new KeyboardButton("ФТ-104-2")
                        },
                    },
                    ResizeKeyboard = true
                };
            }
            

            private IReplyMarkup GetInlineButtons(string id)
            {
                return new InlineKeyboardMarkup(new InlineKeyboardButton(id)
                    { Text = "Добавить", CallbackData = id.ToString() });
            }

            private IReplyMarkup GetMainMenuButtons()
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
                        new List<KeyboardButton> //Вторая строка кнопок
                        {
                            new KeyboardButton(HomeworkStatusButton),
                        },
                    },
                ResizeKeyboard = true
                };
            }
        }
    }
}