namespace TelegramBotExperiments;

public class Images
{
    public static Dictionary<int, string> imageDictionary = new()
    {
        { 0, "file_122593456.png" },
        {1, "file_122593463.png"},
        {2, "file_122593467.png"}
    };

    public static string GetImage(int number)
    {
        return imageDictionary[number];
    }
}