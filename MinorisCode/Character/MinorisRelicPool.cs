using BaseLib.Abstracts;
using Godot;
using Minoris.MinorisCode.Extensions;

namespace Minoris.MinorisCode.Character;

public class MinorisRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Minoris.Color;
    public override string BigEnergyIconPath => "ui/energy/minoris_card_energy_icon.png".ImagePath();
    public override string TextEnergyIconPath => "ui/energy/minoris_energy_icon.png".ImagePath();
}
