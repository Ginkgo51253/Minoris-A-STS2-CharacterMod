using BaseLib.Abstracts;
using Godot;
using Minoris.MinorisCode.Extensions;

namespace Minoris.MinorisCode.Character;

public class MinorisCardPool : CustomCardPoolModel
{
    public override string Title => Minoris.CharacterId;
    public override string BigEnergyIconPath =>
        ResourceLoader.Exists("charui/big_energy.png".ImagePath())
            ? "charui/big_energy.png".ImagePath()
            : "ui/combat/energy_counters/minoris_energy_icon.png".ImagePath();

    public override string TextEnergyIconPath =>
        ResourceLoader.Exists("charui/text_energy.png".ImagePath())
            ? "charui/text_energy.png".ImagePath()
            : "ui/combat/energy_counters/minoris_energy_icon.png".ImagePath();
    public override Color ShaderColor => Minoris.Color;
    public override Color DeckEntryCardColor => Minoris.Color;
    public override Color EnergyOutlineColor => Minoris.EnergyOutlineColor;
    public override bool IsColorless => false;
}
