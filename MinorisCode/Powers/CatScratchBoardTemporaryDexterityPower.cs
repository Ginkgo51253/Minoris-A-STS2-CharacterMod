namespace Minoris.MinorisCode.Powers;

using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;
using Minoris.MinorisCode.Cards;
using Minoris.MinorisCode.Extensions;

public class CatScratchBoardTemporaryDexterityPower : TemporaryDexterityPower, ICustomPower
{
    public override AbstractModel OriginModel => ModelDb.Card<Card063_CatScratchBoard>();

    public string? CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "res://images/powers/missing_power.png";
        }
    }

    public string? CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "res://images/powers/missing_power.png";
        }
    }

    public string? CustomBigBetaIconPath => null;
}
