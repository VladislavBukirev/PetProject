using System.Transactions;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelegramBotExperiments
{
    class Program
    {
        private static string GetToken()
        {
            string token;
            using var reader = new StreamReader("token.txt");
            token = reader.ReadLine();
            return token;
        }
        static void Main(string[] args)
        {
            var token = GetToken();
            
            try
            {
                TelegramBotHelper hlp = new TelegramBotHelper(token: token);
                hlp.GetUpdates();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private class TelegramBotHelper
        {
            private string token = GetToken();
            private Telegram.Bot.TelegramBotClient bot;

            private Dictionary<long, UserState> _clientStates = new Dictionary<long, UserState>();

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
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

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
                        var state = _clientStates.ContainsKey(update.Message.Chat.Id)
                            ? _clientStates[update.Message.Chat.Id]
                            : null;
                        if (state != null)
                        {
                            switch (state.State)
                            {
                                case State.AddHomework:
                                    if (text.Equals(Buttons.TEXT_BACK))
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Откат из меню групп в MainMenu", replyMarkup: GetMainMenuButtons());
                                        _clientStates[update.Message.Chat.Id] = null;
                                        break;
                                    }
                                    if (text.Equals(Buttons.TEXT_BACK2))
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Откат из графы добавления домашнего задания в выбор групп",
                                            replyMarkup: GetGroupButtons());
                                        _clientStates[update.Message.Chat.Id] = new UserState
                                            { State = State.AddHomework };
                                    }
                                    else
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Функция добавления домашнего задания",
                                            replyMarkup: GetAddHomeworkButton());
                                    }
                                    break;
                                
                                case State.StatusHomework:
                                    if (text.Equals(Buttons.TEXT_BACK))
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Откат из меню групп в MainMenu", replyMarkup: GetMainMenuButtons());
                                        _clientStates[update.Message.Chat.Id] = null;
                                        break;
                                    }
                                    if (text.Equals(Buttons.TEXT_BACK2))
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Откат из графы добавления домашнего задания в выбор групп",
                                            replyMarkup: GetGroupButtons());
                                        _clientStates[update.Message.Chat.Id] = new UserState
                                            { State = State.StatusHomework };
                                    }
                                    else
                                    {
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Функция получения домашнего задания", replyMarkup: GetHomeworkButton());
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
                                    case Buttons.GetImgButton:
                                        var rnd = new Random();
                                        image = Path.Combine(Environment.CurrentDirectory,
                                            Images.GetImage(rnd.Next(100))); //достать картинку
                                        using (var stream = System.IO.File.OpenRead(image)) //открыть картинку
                                        {
                                            var r = bot.SendPhotoAsync(update.Message.Chat.Id,
                                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                                replyMarkup: GetMainMenuButtons()).Result;
                                        }
                                        break;
                                    
                                    case Buttons.AddHomeworkButton:
                                        _clientStates[update.Message.Chat.Id] = new UserState
                                            { State = State.AddHomework };
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Переход в иконку добавления домашнего задания",
                                            replyMarkup: GetGroupButtons());
                                        break;

                                    case Buttons.HomeworkStatusButton:
                                        _clientStates[update.Message.Chat.Id] = new UserState
                                            { State = State.StatusHomework };
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Переход в иконку получения домашнего задания",
                                            replyMarkup: GetGroupButtons());
                                        break;

                                    default:
                                        bot.SendTextMessageAsync(update.Message.Chat.Id,
                                            "Дефолтный случай свичкейса " + text,
                                            replyMarkup: GetMainMenuButtons());
                                        break;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            private IReplyMarkup GetGroupButtons()
            {
                return Buttons.GetGroupButtons();
            }

            private IReplyMarkup GetAddHomeworkButton()
            {
                return Buttons.GetAddHomeworkButton();
            }

            private IReplyMarkup GetHomeworkButton()
            {
                return Buttons.GetAddHomeworkButton();
            }

            private IReplyMarkup GetMainMenuButtons()
            {
                return Buttons.GetMainMenuButtons();
            }
        }

        private class UserState
        {
            public State State { get; set; }
        }

        public enum State
        {
            None,
            AddHomework,
            StatusHomework,
        }
    }
}