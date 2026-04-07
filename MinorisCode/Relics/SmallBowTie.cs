using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using System.Linq;

namespace Minoris.MinorisCode.Relics;

[Pool(typeof(DeprecatedRelicPool))]
public class SmallBowTie : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (Owner?.Creature == null) return;
        if (side != Owner.Creature.Side) return;
        if (combatState.RoundNumber > 1) return;
        var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
        if (hand.Count == 0) return;
        var pick = Owner.RunState.Rng.CombatCardGeneration.NextItem(hand);
        CardCmd.Upgrade(pick);
        Flash();
        await Task.CompletedTask;
    }
}
