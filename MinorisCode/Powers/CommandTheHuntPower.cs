
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 号令狂猎
能力英文名称: Command the Hunt
能力描述(ZHS): 本回合中，每当另一名玩家打出一张攻击牌时，额外打出[blue]{Amount}[/blue]次。回合结束时移除。
能力描述(ENG): This turn, whenever another player plays an Attack, play it [blue]{Amount}[/blue] additional time(s). Remove at end of turn.
相关卡牌（本地键）: MINORIS-CARD076_COMMAND_THE_HUNT
*/
public class CommandTheHuntPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature == Owner) return playCount; // 自己打出的卡牌不触发
        if (card.Type != CardType.Attack) return playCount;
        return playCount + Amount;
    }

    public override async Task AfterModifyingCardPlayCount(CardModel card)
    {
        if (card.Owner.Creature == Owner) return; // 自己打出的卡牌不触发
        if (card.Type != CardType.Attack) return;
        Flash();
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}














