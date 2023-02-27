using Telegram.Bots.Requests;
using System.IO;

namespace TelegramBotExperiments;

public class ImageManager
{
    static string[] files = Directory.GetFiles(@"C:\Users\79521\Desktop\git-actually\helloapp-black\PetProject\PetProject\fiitsosatb_png\fiitsosatb\png", "*.png", SearchOption.AllDirectories);

    public static string GetImage(int number)
    {
        return files[number];
    }
}