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
            private const string TEXT_BACK = "Назад";
            private Dictionary<long, UserState> _clientStates = new Dictionary<long, UserState>();
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
                        var state = _clientStates.ContainsKey(update.Message.Chat.Id) ? _clientStates[update.Message.Chat.Id] : null;
                        if (state != null)
                        {
                            switch (state.State)
                            {
                                case State.AddHomework :
                                    // if (text.Equals(TEXT_BACK))
                                    // {
                                    //     bot.SendTextMessageAsync(update.Message.Chat.Id, "Выберите:", replyMarkup: GetMainMenuButtons());
                                    //     _clientStates[update.Message.Chat.Id] = null;
                                    // }
                                    // else
                                    {
                                        // bot.SendTextMessageAsync(update.Message.Chat.Id, "Выберите вашу группу", replyMarkup: GetGroupButtons());
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Добавляй, шо ждёшь?", replyMarkup: GetAddHomeworkButton());
                                        switch (text)
                                        {
                                            case TEXT_BACK:
                                                bot.SendTextMessageAsync(update.Message.Chat.Id, "Выберите:", replyMarkup: GetGroupButtons());
                                                _clientStates[update.Message.Chat.Id] = null;
                                                break;
                                        }
                                    }
                                    break;
                                case State.StatusHomework :
                                    if (text.Equals(TEXT_BACK))
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Выберите:", replyMarkup: GetMainMenuButtons());
                                        _clientStates[update.Message.Chat.Id] = null;
                                    }
                                    else
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Выберите вашу группу", replyMarkup: GetGroupButtons());
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Куда блет эти ветки идут", replyMarkup: GetGroupButtons());
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            {
                                string image;
                                switch (text)
                                {
                                    // case TEXT_BACK:
                                    //     bot.SendTextMessageAsync(update.Message.Chat.Id, "Пиздуй", replyMarkup: GetMainMenuButtons());
                                    //     _clientStates[update.Message.Chat.Id] = null;
                                    //     break;
                                    case GetImgButton:
                                        var rnd = new Random();
                                        image = Path.Combine(Environment.CurrentDirectory,
                                            Images.GetImage(rnd.Next(3))); //достать картинку
                                        using (var stream = System.IO.File.OpenRead(image)) //открыть картинку
                                        {
                                            var r = bot.SendPhotoAsync(update.Message.Chat.Id,
                                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                                replyMarkup: GetMainMenuButtons()).Result;
                                        }

                                        break;
                                    case AddHomeworkButton:
                                        _clientStates[update.Message.Chat.Id] = new UserState
                                            { State = State.AddHomework };
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите автора:",
                                            replyMarkup: GetGroupButtons());
                                        image = Path.Combine(Environment.CurrentDirectory,
                                            "101.png"); //достать картинку
                                        using (var stream = System.IO.File.OpenRead(image)) //открыть картинку
                                        {
                                            var r = bot.SendPhotoAsync(update.Message.Chat.Id,
                                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                                replyMarkup: GetInlineButtons("1")).Result;
                                        }

                                        break;
                                    case HomeworkStatusButton:
                                        var dayy = DateTime.Today;
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "HomeworkDict[\"ФТ-103-2\"][dayy.ToString()][0]",
                                            replyMarkup: GetGroupButtons());
                                        break;
                                    default:
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Recieved text: " + text,
                                            replyMarkup: GetMainMenuButtons());
                                        break;
                                }
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
                        new List<KeyboardButton>()
                        {
                            new KeyboardButton(TEXT_BACK)
                        },
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

            private IReplyMarkup GetAddHomeworkButton()
            {
                return new ReplyKeyboardMarkup(token)
                {
                    Keyboard = new List<List<KeyboardButton>>()
                    {
                        new List<KeyboardButton>()
                        {
                            new KeyboardButton("Добавить домашнее задание"),
                            new KeyboardButton(TEXT_BACK)
                        }
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

    public class UserState
    {
        public State State { get; set; }
    }

    public enum State
    {
        None,
        AddHomework,
        StatusHomework
    }
}