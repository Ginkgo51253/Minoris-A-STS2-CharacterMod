using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;

namespace Minoris.MinorisCode.Relics;

public class BandageRoll : MinorisRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _isOwnersTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        _isOwnersTurn = player == Owner;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Enemy)
        {
            _isOwnersTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner?.Creature == null) return;
        if (target != Owner.Creature) return;
        if (!_isOwnersTurn) return;
        if (result.UnblockedDamage <= 0) return;
        await CreatureCmd.Heal(Owner.Creature, 1m);
        Flash();
    }
}
