using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExperiments;

// Напиши названия классов более явно. ButtonManager какой-нибудь
public class Buttons
{
    // Не нужно здесь хранить токен
    private static string token = "6271910487:AAH2yNNXNTPtVr5azFv_HcGl6haBZuAiuws";
    
    // Строки лучше сразу выносить в файлы локализации https://learn.microsoft.com/en-us/dotnet/core/extensions/localization
    public const string GetImgButton = "Хочу картинку";
            
    public const string AddHomeworkButton = "Добавить домашнее задание";
    public const string HomeworkStatusButton = "Узнать состояние домашнего задания";
            
    private const string AddHomeworkInGroupButton = "Добавить домашнее задание для моей группы";
    private const string GetHomeworkInGroupButton = "Узнать домашнее задание для моей группы";
            
    public const string TEXT_BACK = "Откат из меню групп в MainMenu";
    public const string TEXT_BACK2 = "Откат из графы добавления домашнего задания в выбор групп";
    
    public static IReplyMarkup GetMainMenuButtons()
    {
        return new ReplyKeyboardMarkup(token) //Передаём в клавиатуру токен нашего бота
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton> //Первая строка кнопок
                {
                    new KeyboardButton(GetImgButton),
                },
                new List<KeyboardButton> //Вторая строка кнопок
                {
                    new KeyboardButton(AddHomeworkButton),
                    new KeyboardButton(HomeworkStatusButton),
                },
            },
            ResizeKeyboard = true
        };
    }
    
    public static IReplyMarkup GetAddHomeworkButton()
    {
        return new ReplyKeyboardMarkup(token)
        {
            Keyboard = new List<List<KeyboardButton>>()
            {
                new List<KeyboardButton>()
                {
                    new KeyboardButton(AddHomeworkInGroupButton),
                    new KeyboardButton(TEXT_BACK2)
                }
            },
            ResizeKeyboard = true
        };
    }
    
    public static IReplyMarkup GetHomeworkButton()
    {
        return new ReplyKeyboardMarkup(token)
        {
            Keyboard = new List<List<KeyboardButton>>()
            {
                new List<KeyboardButton>()
                {
                    new KeyboardButton(GetHomeworkInGroupButton),
                    new KeyboardButton(TEXT_BACK2)
                }
            },
            ResizeKeyboard = true
        };
    }
    
    public static IReplyMarkup GetGroupButtons()
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
}