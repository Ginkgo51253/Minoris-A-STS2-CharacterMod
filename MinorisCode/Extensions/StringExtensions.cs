namespace Minoris.MinorisCode.Extensions;

public static class StringExtensions
{
    private static string ResPath(params string[] segments)
    {
        return "res://" + string.Join("/", segments).Replace('\\', '/');
    }

    public static string ImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", path);
    }
    
    public static string CardImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "card_portraits", path);
    }
    public static string BigCardImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "card_portraits", "big", path);
    }

    public static string PowerImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "powers", "big", path);
    }

    public static string RelicImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "relics", "big", path);
    }

    public static string CharacterUiPath(this string path)
    {
        return ResPath(MainFile.ModId, "images", "charui", path);
    }
}
