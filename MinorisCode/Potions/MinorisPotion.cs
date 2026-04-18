namespace Minoris.MinorisCode.Potions;

[BaseLib.Utils.Pool(typeof(MinorisPotionPool))]
public abstract class MinorisPotion : CustomPotionModel
{
    private const string MissingPotionIconPath = "res://images/powers/missing_power.png";

    public override string? CustomPackedImagePath
    {
        get
        {
            var atlasPath = $"res://atlases/potion_atlas.sprites/{Id.Entry.ToLowerInvariant()}.tres";
            if (ResourceLoader.Exists(atlasPath)) return atlasPath;
            var pngPath = $"potions/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ImagePath();
            return ResourceLoader.Exists(pngPath) ? pngPath : MissingPotionIconPath;
        }
    }

    public override string? CustomPackedOutlinePath
    {
        get
        {
            var outlineAtlasPath = $"res://atlases/potion_outline_atlas.sprites/{Id.Entry.ToLowerInvariant()}.tres";
            if (ResourceLoader.Exists(outlineAtlasPath)) return outlineAtlasPath;
            var pngOutline = $"potions/{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".ImagePath();
            if (ResourceLoader.Exists(pngOutline)) return pngOutline;
            return MissingPotionIconPath;
        }
    }
}
