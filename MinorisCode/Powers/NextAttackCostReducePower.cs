
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 攻击减费
能力英文名称: Next Attack Cost Reduce
能力描述(ZHS): 本回合中，你下一张攻击牌的费用减少[blue]{Amount}[/blue]点。然后移除。回合结束时移除。
能力描述(ENG): This turn, your next Attack costs [blue]{Amount}[/blue] less. Then remove this. Remove at end of turn.
相关卡牌（本地键）: MINORIS-CARD044_MINT_CANDY
*/
public class NextAttackCostReducePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner) return false;
        if (card.Type != CardType.Attack) return false;
        if (originalCost <= 0m) return false;
        modifiedCost = originalCost - Amount;
        if (modifiedCost < 0m) modifiedCost = 0m;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        RemoveInternal();
        await Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}














