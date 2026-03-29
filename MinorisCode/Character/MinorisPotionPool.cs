using BaseLib.Abstracts;
using Godot;
using Minoris.MinorisCode.Extensions;

namespace Minoris.MinorisCode.Character;

public class MinorisPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Minoris.Color;
    public override string BigEnergyIconPath =>
        ResourceLoader.Exists("charui/big_energy.png".ImagePath())
            ? "charui/big_energy.png".ImagePath()
            : "ui/combat/energy_counters/minoris_energy_icon.png".ImagePath();

    public override string TextEnergyIconPath =>
        ResourceLoader.Exists("charui/text_energy.png".ImagePath())
            ? "charui/text_energy.png".ImagePath()
            : "ui/combat/energy_counters/minoris_energy_icon.png".ImagePath();
}
