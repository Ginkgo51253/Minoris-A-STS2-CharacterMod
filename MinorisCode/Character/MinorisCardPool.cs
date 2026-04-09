using BaseLib.Abstracts;
using Godot;
using Minoris.MinorisCode.Extensions;

namespace Minoris.MinorisCode.Character;

public class MinorisCardPool : CustomCardPoolModel
{
    public override string Title => Minoris.CharacterId;
    public override string BigEnergyIconPath => "ui/energy/minoris_card_energy_icon.png".ImagePath();
    public override string TextEnergyIconPath => "ui/energy/minoris_energy_icon.png".ImagePath();
    public override Color ShaderColor => Minoris.Color;
    public override Color DeckEntryCardColor => Minoris.Color;
    public override Color EnergyOutlineColor => Minoris.EnergyOutlineColor;
    public override bool IsColorless => false;
}
