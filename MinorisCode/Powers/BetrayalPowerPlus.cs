namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 背叛+
能力英文名称: Betrayal+
能力描述(ZHS): 在你的回合开始时，从其他职业的卡牌中随机展示3张升级过的卡牌，选择1张置入你的手牌。它在打出前为0费。
能力描述(ENG): At the start of your turn, choose 1 of 3 upgraded random cards from other classes. It costs 0 until played.
相关卡牌（本地键）: MINORIS-CARD069_3_BETRAYAL
*/
public class BetrayalPowerPlus : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

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
            .Where(c => c.IsUpgradable)
            .ToList();
        if (candidates.Count == 0) return;
        var rng = player.RunState.Rng.CombatCardGeneration;

        async Task OfferOnce()
        {
            if (CombatManager.Instance.IsEnding) return;
            var sourcePool = candidates;
            if (sourcePool.Count == 0) return;
            var options = new List<CardModel>();
            var optionCount = System.Math.Min(3, sourcePool.Count);
            for (var i = 0; i < optionCount; i++)
            {
                var pick = rng.NextItem(sourcePool);
                if (pick == null) break;
                sourcePool.Remove(pick);
                var card = CombatState.CreateCard(pick, Owner.Player);
                CardCmd.Upgrade(card);
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

        for (var i = 0; i < Amount; i++)
        {
            await OfferOnce();
        }
    }
}
