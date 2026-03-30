
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 背叛
能力英文名称: Betrayal
能力描述(ZHS): 在你的回合开始时，从其他职业的卡牌中随机展示3张，选择1张置入你的手牌。它在打出前为0费。
能力描述(ENG): At the start of your turn, choose 1 of 3 random cards from other classes. It costs 0 until played.
相关卡牌（本地键）: MINORIS-CARD069_3_BETRAYAL
*/
public class BetrayalPower : MinorisPower
{
    private class Data
    {
        public int upgradedAmount;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int UpgradedAmount => GetInternalData<Data>().upgradedAmount;
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var normalTimes = System.Math.Max(0, Amount - UpgradedAmount);
            var upgradedTimes = System.Math.Max(0, UpgradedAmount);
            return
            [
                new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-BETRAYAL").ToString() ?? string.Empty)
                    .Replace("{NormalTimes}", normalTimes.ToString())
                    .Replace("{UpgradedTimes}", upgradedTimes.ToString()), isSmart: false)
            ];
        }
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.IsUpgraded)
        {
            GetInternalData<Data>().upgradedAmount = Amount;
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount != 0m && cardSource != null && cardSource.IsUpgraded)
        {
            GetInternalData<Data>().upgradedAmount = System.Math.Max(0, GetInternalData<Data>().upgradedAmount + (int)amount);
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || CombatState == null) return;
        var myPoolTitle = player.Character.CardPool.Title;
        var otherPools = player.UnlockState.CharacterCardPools
            .Where(p => p.Title != myPoolTitle)
            .ToList();
        var candidates = otherPools
            .SelectMany(p => p.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint))
            .Where(c => c.Rarity != CardRarity.Token && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event)
            .ToList();
        if (candidates.Count == 0) return;
        var rng = player.RunState.Rng.CombatCardGeneration;

        async Task OfferOnce(bool upgradeOfferedCards)
        {
            if (CombatManager.Instance.IsEnding) return;
            if (candidates.Count == 0) return;
            var options = new List<CardModel>();
            var optionCount = System.Math.Min(3, candidates.Count);
            for (var i = 0; i < optionCount; i++)
            {
                var pick = rng.NextItem(candidates);
                if (pick == null) break;
                candidates.Remove(pick);
                var card = CombatState.CreateCard(pick, Owner.Player);
                if (upgradeOfferedCards) CardCmd.Upgrade(card);
                card.EnergyCost.SetUntilPlayed(0);
                options.Add(card);
            }
            if (options.Count == 0) return;
            if (CombatManager.Instance.IsEnding) return;
            var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner.Player);
            if (chosen == null) return;
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(chosen, PileType.Hand, false, CardPilePosition.Top));
            Flash();
        }

        var normalTimes = System.Math.Max(0, Amount - UpgradedAmount);
        var upgradedTimes = System.Math.Max(0, UpgradedAmount);

        for (var i = 0; i < normalTimes; i++)
        {
            await OfferOnce(false);
        }

        for (var i = 0; i < upgradedTimes; i++)
        {
            await OfferOnce(true);
        }
    }
}














