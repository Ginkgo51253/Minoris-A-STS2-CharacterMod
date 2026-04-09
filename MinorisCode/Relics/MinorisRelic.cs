namespace Minoris.MinorisCode.Relics;

[BaseLib.Utils.Pool(typeof(MinorisRelicPool))]
public abstract class MinorisRelic : CustomRelicModel
{
    private const string MissingRelicIconPath = "res://images/powers/missing_power.png";

    public override string PackedIconPath
    {
        get
        {
            var atlas = $"res://atlases/relic_atlas.sprites/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres";
            if (ResourceLoader.Exists(atlas)) return atlas;
            var explicitPath = $"{Id.Entry}_S.png".RelicImagePath();
            if (ResourceLoader.Exists(explicitPath)) return explicitPath;

            var legacyPath = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(legacyPath) ? legacyPath : MissingRelicIconPath;
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var atlas = $"res://atlases/relic_outline_atlas.sprites/{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres";
            if (ResourceLoader.Exists(atlas)) return atlas;
            var explicitPath = $"{Id.Entry}_S_outline.png".RelicImagePath();
            if (ResourceLoader.Exists(explicitPath)) return explicitPath;

            var legacyPath = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(legacyPath) ? legacyPath : MissingRelicIconPath;
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var explicitPath = $"{Id.Entry}_B.png".BigRelicImagePath();
            if (ResourceLoader.Exists(explicitPath)) return explicitPath;

            var legacyPath = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            if (ResourceLoader.Exists(legacyPath)) return legacyPath;

            var smallExplicitPath = $"{Id.Entry}_S.png".RelicImagePath();
            if (ResourceLoader.Exists(smallExplicitPath)) return smallExplicitPath;

            var smallLegacyPath = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(smallLegacyPath) ? smallLegacyPath : MissingRelicIconPath;
        }
    }
}
