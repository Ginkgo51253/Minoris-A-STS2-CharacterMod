using Minoris.MinorisCode.Extensions;

namespace Minoris.MinorisCode.Cards;

[Pool(typeof(MinorisCardPool))]
public abstract class MinorisCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    private const string DefaultCardPortraitPath = "res://images/packed/card_portraits/beta.png";

    private string AtlasPortraitPath(bool beta)
    {
        var poolTitle = Pool.Title.ToLowerInvariant();
        var id = Id.Entry.ToLowerInvariant();
        var stem = GetCardPortraitFileStem()?.ToLowerInvariant();
        if (beta)
        {
            if (stem != null)
            {
                var byStem = $"res://atlases/card_atlas.sprites/{poolTitle}/beta/{stem}.tres";
                if (ResourceLoader.Exists(byStem)) return byStem;
            }
            return $"res://atlases/card_atlas.sprites/{poolTitle}/beta/{id}.tres";
        }
        else
        {
            if (stem != null)
            {
                var byStem = $"res://atlases/card_atlas.sprites/{poolTitle}/{stem}.tres";
                if (ResourceLoader.Exists(byStem)) return byStem;
            }
            return $"res://atlases/card_atlas.sprites/{poolTitle}/{id}.tres";
        }
    }

    private string? GetCardPortraitFileStem()
    {
        var name = GetType().Name;
        if (!name.StartsWith("Card", StringComparison.Ordinal)) return null;
        if (name.Length < 7) return null;
        for (var i = 4; i < 7; i++)
        {
            var c = name[i];
            if (c < '0' || c > '9') return null;
        }
        var number = name.Substring(4, 3);
        var stem = $"MINORIS-CARD-{number}";

        if (name.Length >= 9 && name[7] == '_' && name[8] >= '0' && name[8] <= '9')
        {
            var end = 8;
            while (end < name.Length)
            {
                var c = name[end];
                if (c < '0' || c > '9') break;
                end++;
            }
            var tokenIndex = name.Substring(8, end - 8);
            if (tokenIndex.Length > 0)
            {
                stem = $"{stem}-{tokenIndex}";
            }
        }

        return stem;
    }

    private string? ResolveNumberedPortraitPath(bool big, bool beta)
    {
        var stem = GetCardPortraitFileStem();
        if (stem == null) return null;
        var fileName = $"{stem}.png";
        if (big)
        {
            var bigPath = fileName.BigCardImagePath();
            if (ResourceLoader.Exists(bigPath)) return bigPath;
        }

        var basePath = beta
            ? $"beta/{fileName}".CardImagePath()
            : fileName.CardImagePath();
        return ResourceLoader.Exists(basePath) ? basePath : null;
    }

    public override string CustomPortraitPath =>
        ResolveNumberedPortraitPath(big: true, beta: false) ??
        ResolveNumberedPortraitPath(big: false, beta: false) ??
        DefaultCardPortraitPath;

    public override string PortraitPath
    {
        get
        {
            var atlas = AtlasPortraitPath(beta: false);
            if (ResourceLoader.Exists(atlas)) return atlas;
            return ResolveNumberedPortraitPath(big: false, beta: false) ?? DefaultCardPortraitPath;
        }
    }

    public override string BetaPortraitPath
    {
        get
        {
            var atlas = AtlasPortraitPath(beta: true);
            if (ResourceLoader.Exists(atlas)) return atlas;
            return ResolveNumberedPortraitPath(big: false, beta: true) ?? PortraitPath;
        }
    }
}
