namespace TelegramBotExperiments;

public class Images
{
    private static Dictionary<int, string> imageDictionary = new();
    private static void FillDictionary()
    {
        for(var i = 0; i < 100; i++)
        {
            imageDictionary[i] = $"file_122593{456 + i}.png";
        }
    }

    public static string GetImage(int number)
    {
        FillDictionary();
        return imageDictionary[number];
    }
}