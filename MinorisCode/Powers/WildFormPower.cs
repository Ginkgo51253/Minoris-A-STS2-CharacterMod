﻿
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
    private bool _subscribed;

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && Amount != 1)
        {
            Amount = 1;
        }
        if (power == this && !_subscribed && Owner?.Player != null && CombatManager.Instance.IsInProgress)
        {
            var player = Owner.Player;
            ApplyToExistingAttacks(player);
            SubscribePileAdds(player);
            _subscribed = true;
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

    private static void ProcessIfAttack(CardModel card)
    {
        if (card.Type != CardType.Attack) return;
        card.EnergyCost.SetThisCombat(0);
        card.AddKeyword(CardKeyword.Ethereal);
    }

    private static void ApplyToExistingAttacks(Player player)
    {
        foreach (var c in CardPile.GetCards(player, PileType.Draw, PileType.Hand, PileType.Discard))
        {
            ProcessIfAttack(c);
        }
    }

    private void SubscribePileAdds(Player player)
    {
        var draw = PileType.Draw.GetPile(player);
        var hand = PileType.Hand.GetPile(player);
        var discard = PileType.Discard.GetPile(player);
        if (draw != null) draw.CardAdded += ProcessIfAttack;
        if (hand != null) hand.CardAdded += ProcessIfAttack;
        if (discard != null) discard.CardAdded += ProcessIfAttack;
    }
}














