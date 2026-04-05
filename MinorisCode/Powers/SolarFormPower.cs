namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 太阳形态
能力英文名称: Solar Form
能力描述(ZHS): 在你的回合开始时，随机将一张攻击牌、一张技能牌、一张能力牌置入你的手中。它们在本场战斗中为0费，但具有虚无。
能力描述(ENG): At the start of your turn, add a random Attack, Skill, and Power to your hand. They cost 0 this combat, but are Ethereal.
相关卡牌（本地键）: MINORIS-CARD071_SOLAR_FORM
*/
public class SolarFormPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-SOLAR_FORM").ToString() ?? string.Empty), isSmart: false)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || CombatState == null) return;
        var all = player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c.CanBeGeneratedInCombat)
            .Where(c => c.Rarity != CardRarity.Basic && c.Rarity != CardRarity.Ancient && c.Rarity != CardRarity.Event && c.Rarity != CardRarity.Token)
            .ToList();
        
        var pickA = all.Where(c => c.Type == CardType.Attack).ToList();
        var pickS = all.Where(c => c.Type == CardType.Skill).ToList();
        var pickP = all.Where(c => c.Type == CardType.Power).ToList();
        if (pickA.Count == 0 || pickS.Count == 0 || pickP.Count == 0) return;
        
        var rng = player.RunState.Rng.CombatCardGeneration;
        var countPerType = System.Math.Max(1, (int)Amount);

        async Task CreateAndAdd(IReadOnlyList<CardModel> pool, bool upgradeCard)
        {
            var canonical = rng.NextItem(pool);
            if (canonical == null) return;
            var card = CombatState.CreateCard(canonical, Owner.Player);
            if (upgradeCard) CardCmd.Upgrade(card);
            card.EnergyCost.SetThisCombat(0);
            card.AddKeyword(CardKeyword.Ethereal);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, false, CardPilePosition.Top));
        }

        for (var i = 0; i < countPerType; i++)
        {
            await CreateAndAdd(pickA, false);
        }
        for (var i = 0; i < countPerType; i++)
        {
            await CreateAndAdd(pickS, false);
        }
        for (var i = 0; i < countPerType; i++)
        {
            await CreateAndAdd(pickP, false);
        }

        Flash();
    }
}
