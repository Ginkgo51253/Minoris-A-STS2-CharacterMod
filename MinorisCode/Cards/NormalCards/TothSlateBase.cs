namespace Minoris.MinorisCode.Cards;


/*
本地键: 
中文名称: 
英文名称: 
卡牌类型: CardType.Skill
卡牌稀有度: rarity
tag标签: 
关键字: CardKeyword.Exhaust, CardKeyword.Ethereal
费用: 3
卡牌描述(ZHS): 
卡牌描述(ENG): 
升级效果(代码): EnergyCost.UpgradeBy(-1);
*/
public abstract class TothSlateBase<TNext>(CardRarity rarity, TargetType targetType) :
    MinorisCard(3, CardType.Skill, rarity, targetType)
    where TNext : CardModel
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];
    public override bool ShouldReceiveCombatHooks => true;
    private bool _wasPlayedThisCombat;
    private int _upgradeLevelWhenPlayed;

    protected abstract int Stage { get; }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _wasPlayedThisCombat = true;
        _upgradeLevelWhenPlayed = CurrentUpgradeLevel;
        await ApplyStageEffects(choiceContext, cardPlay);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (!_wasPlayedThisCombat) return;
        if (DeckVersion == null) return;
        var result = await CardCmd.TransformTo<TNext>(DeckVersion);
        if (result == null) return;
        var added = result.Value.cardAdded;

        while (_upgradeLevelWhenPlayed > 0 && added.IsUpgradable)
        {
            CardCmd.Upgrade(added);
            _upgradeLevelWhenPlayed--;
        }
    }

    protected virtual async Task ApplyStageEffects(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Stage >= 2)
        {
            await CreatureCmd.GainBlock(Owner.Creature, 5m, ValueProp.Move, cardPlay);
        }

        if (Stage >= 3)
        {
            if (cardPlay.Target != null)
            {
                await DamageCmd.Attack(5m).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
            }
        }

        if (Stage >= 4)
        {
            await PlayerCmd.GainEnergy(1, Owner);
        }

        if (Stage >= 5)
        {
            await CreatureCmd.Heal(Owner.Creature, 2m);
        }

        if (Stage >= 6)
        {
            var hand = PileType.Hand.GetPile(Owner).Cards.Where(c => c != this).ToList();
            if (hand.Count > 0)
            {
                var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 0, 1);
                var pick = (await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, prefs)).FirstOrDefault();
                if (pick != null) await CardCmd.Exhaust(choiceContext, pick);
            }
        }

        if (Stage >= 7)
        {
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        if (Stage >= 8)
        {
            var drawn = await CardPileCmd.Draw(choiceContext, Owner);
            if (drawn != null)
            {
                CardCmd.ApplyKeyword(drawn, CardKeyword.Retain);
            }
        }

        if (Stage >= 9)
        {
            await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        if (Stage >= 10)
        {
            await PowerCmd.Apply<VigorPower>(Owner.Creature, 4, Owner.Creature, this);
        }

        if (Stage >= 11)
        {
            await PowerCmd.Apply<ArtifactPower>(Owner.Creature, 1, Owner.Creature, this);
        }

        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}

