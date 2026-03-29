using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using Minoris.MinorisCode.Character;
using Minoris.MinorisCode.Extensions;
using System;
using System.IO;

namespace Minoris.MinorisCode.Cards;

[Pool(typeof(MinorisCardPool))]
public abstract class MinorisCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    private const string DefaultCardPortraitPath = "res://images/packed/card_portraits/beta.png";

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

    public override string PortraitPath =>
        ResolveNumberedPortraitPath(big: false, beta: false) ??
        DefaultCardPortraitPath;

    public override string BetaPortraitPath =>
        ResolveNumberedPortraitPath(big: false, beta: true) ??
        PortraitPath;
}
