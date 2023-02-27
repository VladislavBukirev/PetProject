using Telegram.Bots.Requests;
using System.IO;

namespace TelegramBotExperiments;

public class ImageManager
{
    static string[] files = Directory.GetFiles($@"{Directory.GetCurrentDirectory().Split("PetProject")[0]}\PetProject\fiitsosatb_png\fiitsosatb\png",
        "*.png", SearchOption.AllDirectories);

    public static string GetImage(int number)
    {
        return files[number];
    }
}