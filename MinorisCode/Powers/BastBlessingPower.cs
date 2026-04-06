
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 芭丝特赐福
能力英文名称: Bast's Blessing
能力描述(ZHS): 你的攻击牌会额外打出[blue]{Amount}[/blue]次。
能力描述(ENG): Your Attacks are played [blue]{Amount}[/blue] additional time(s).
相关卡牌（本地键）: MINORIS-CARD072_BAST_BLESSING
*/
public class BastBlessingPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner != Owner.Player) return playCount;
        if (card.Type != CardType.Attack) return playCount;
        return playCount + Amount;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        if (card.Owner == Owner.Player && card.Type == CardType.Attack) Flash();
        return Task.CompletedTask;
    }
}














