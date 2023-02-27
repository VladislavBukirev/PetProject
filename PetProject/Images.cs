namespace TelegramBotExperiments;

// То же самое, что с Buttons
public class Images
{
    private static Dictionary<int, string> imageDictionary = new();
    // Можно же просто зачитывать все картинки из папки и даже не хранить их в словаре. Ну и картинок нет в репозитории, я не могу собрать и сразу запустить бота
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