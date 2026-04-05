namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD062_1_SCRATCH
中文名称: 抓挠
英文名称: Scratch
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Token
tag标签: 
费用: 0
卡牌效果: 造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。
卡牌描述(ENG): Deal {Damage:diff()} damage {Hits:diff()} times.
升级效果: 造成{Damage:diff()}点伤害次+2
*/
[Pool(typeof(TokenCardPool))]
public class Card062_1_Scratch() : MinorisCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust)];

    private const string HitsKey = "Hits";

    public override TargetType TargetType
    {
        get
        {
            if (CombatManager.Instance.IsInProgress && Owner != null && Owner.Creature.HasPower<Powers.DimensionalScratchPower>())
                return TargetType.AllEnemies;
            return base.TargetType;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(1m, ValueProp.Move),
        new IntVar(HitsKey, 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hits = DynamicVars[HitsKey].IntValue;
        if (CombatState != null && TargetType == TargetType.AllEnemies)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(hits).Execute(choiceContext);
            return;
        }
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(2m);
    }
}


