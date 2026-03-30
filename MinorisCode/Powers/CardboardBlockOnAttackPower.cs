
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 纸壳屋
能力英文名称: Cardboard House
能力描述(ZHS): 本回合中，每当你打出一张攻击牌时，获得[blue]{Amount}[/blue]点格挡。回合结束时移除。
能力描述(ENG): This turn, whenever you play an Attack, gain [blue]{Amount}[/blue] Block. Remove at end of turn.
相关卡牌（本地键）: 
*/
public class CardboardBlockOnAttackPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}














