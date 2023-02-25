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
            private Dictionary<long, UserState> clientStates = new Dictionary<long, UserState>();

            const string GetImgButton = "Хочу картинку";
            const string AddHomeworkButton = "Добавить домашнее задание";
            const string MusicButton = "Music";
            const string Back = "Back";

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
                        var state = clientStates.ContainsKey(update.Message.Chat.Id)
                            ? clientStates[update.Message.Chat.Id] : null;
                        if (state != null)
                        {
                            switch (state.State)
                            {
                                case State.SearchMusic:
                                    List<string> songs = GetSongsByAuthor(author: text);
                                    if (songs != null && songs.Count > 0)
                                    {
                                        state.State = State.SearchSong;
                                        state.Author = text;
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите название песни: ",
                                            replyMarkup: GetSongsButton(songs));
                                    }
                                    else
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Ничего не найдено. Введите автора",
                                            replyMarkup: GetAutors());
                                    }
                                    break;
                                case State.SearchSong:
                                    var songPath = GetSongPath(text);
                                    if (!string.IsNullOrEmpty(songPath))
                                    {
                                        List<string> songs2 = GetSongsByAuthor(author: state.Author);
                                        using (var stream = System.IO.File.OpenRead(songPath)) //открыть картинку
                                        {
                                            var r = bot.SendAudioAsync(update.Message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream), replyMarkup: GetSongsButton(songs2)).Result;
                                        }
                                    }
                                    else
                                    {
                                        List<string> songs2 = GetSongsByAuthor(author: state.Author);
                                        bot.SendTextMessageAsync(update.Message.Chat.Id, "Ничего не найдено. Введите название песни: ",
                                            replyMarkup: GetSongsButton(songs2));
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (text)
                            {
                                case MusicButton: //При нажатии кнопки music создаём для клиента объект (состояние его чата)
                                    clientStates[update.Message.Chat.Id] = new UserState { State = State.SearchMusic };
                                    bot.SendTextMessageAsync(update.Message.Chat.Id, "Введите автора",
                                        replyMarkup: GetAutors());
                                    break;

                                case GetImgButton:
                                    var rnd = new Random();
                                    image = Path.Combine(Environment.CurrentDirectory, Images.GetImage(rnd.Next(3))); //достать картинку
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

            private string GetSongPath(string text)
            {
                if (text.Equals("Биба"))
                {
                    return Path.Combine(Environment.CurrentDirectory, "povezlo-povezlo.mp3");
                }
                else
                    return null;
            }

            private IReplyMarkup GetSongsButton(List<string> songs)
            {
                var Keyboard = new List<List<KeyboardButton>>();
                songs.ForEach(s => Keyboard.Add(new List<KeyboardButton>(){new KeyboardButton(s)}));
                return new ReplyKeyboardMarkup(token)
                {
                    Keyboard = Keyboard,
                    ResizeKeyboard = true
                };
            }

            private List<string> GetSongsByAuthor(string? author)
            {
                if (author.Equals("Biba"))
                    return new List<string>() {"povezlo-povezlo"};
                return null;
            }

            private IReplyMarkup GetAutors()
            {
                return new ReplyKeyboardMarkup(token) //Передаём в клавиатуру токен нашего бота
                {
                    Keyboard = new List<List<KeyboardButton>>
                    {
                        new List<KeyboardButton> //Первая строка кнопок
                        {
                            new KeyboardButton("Биба"),
                            new KeyboardButton("Боба")
                        },
                    },
                    ResizeKeyboard = true
                };
            }

            private class UserState
            {
                public State State { get; set; }
                public string? Author { get; set; }
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
                        new List<KeyboardButton> //Вторая строка кнопок
                        {
                            new KeyboardButton(MusicButton),
                        },
                    },
                ResizeKeyboard = true
                };
            }
        }
    }

    internal enum State
    {
        SearchSong,
        SearchMusic
    }
}