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

    private string? GetCardNumber()
    {
        var name = GetType().Name;
        if (!name.StartsWith("Card", StringComparison.Ordinal)) return null;
        if (name.Length < 7) return null;
        for (var i = 4; i < 7; i++)
        {
            var c = name[i];
            if (c < '0' || c > '9') return null;
        }
        return name.Substring(4, 3);
    }

    private string? ResolveNumberedPortraitPath(bool big, bool beta)
    {
        var number = GetCardNumber();
        if (number == null) return null;
        var fileName = $"MINORIS-CARD-{number}.png";
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
