using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using Minoris.MinorisCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Minoris.MinorisCode.Relics;

public class FreezeDried : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner?.Creature == null) return;
        if (player != Owner) return;
        await PowerCmd.Apply<VigorPower>(Owner.Creature, 4, Owner.Creature, null);
        Flash();
    }
}
