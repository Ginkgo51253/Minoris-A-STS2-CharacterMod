
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 狂野形态
能力英文名称: Wild Form
能力描述(ZHS): 本场战斗中，你的攻击牌费用变为0并具有虚无。
能力描述(ENG): Your Attacks cost 0 this combat and are Ethereal.
相关卡牌（本地键）: MINORIS-CARD075_WILD_FORM
*/
public class WildFormPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && Amount != 1)
        {
            Amount = 1;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        var hand = PileType.Hand.GetPile(player).Cards.ToList();
        foreach (var c in hand)
        {
            if (c.Type != CardType.Attack) continue;
            c.EnergyCost.SetThisCombat(0);
            c.AddKeyword(CardKeyword.Ethereal);
        }
        await Task.CompletedTask;
    }
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel drawnCard, bool fromHandDraw)
    {
        if (drawnCard.Owner != Owner.Player) return;
        if (drawnCard.Type != CardType.Attack) return;
        drawnCard.EnergyCost.SetThisCombat(0);
        drawnCard.AddKeyword(CardKeyword.Ethereal);
        await Task.CompletedTask;
    }
}














