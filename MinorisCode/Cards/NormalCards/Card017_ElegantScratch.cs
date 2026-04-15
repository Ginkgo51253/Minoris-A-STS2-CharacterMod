
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD017_ELEGANT_SCRATCH
中文名称: 优雅抓挠
英文名称: Elegant Scratch
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 造成 2 点伤害 2 次。本场战斗中所有“抓挠”攻击牌造成的伤害增加 {ScratchAmp:diff()} 点。
卡牌描述(ZHS): 造成 2 点伤害 2 次。本场战斗中所有“抓挠”攻击牌造成的伤害增加 {ScratchAmp:diff()} 点。
卡牌描述(ENG): Deal 2 damage 2 times. This combat, "Scratch" Attacks deal +{ScratchAmp:diff()} damage.
升级效果: 本场战斗中所有“抓挠”攻击牌造成的伤害增加点+1
*/
public class Card017_ElegantScratch() : MinorisCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const string ScratchAmpKey = "ScratchAmp";
    private const string DamageKey = "Damage";
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

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var amp = DynamicVars[ScratchAmpKey].IntValue;
        var damage = DynamicVars[DamageKey].BaseValue;
        var hits = DynamicVars[HitsKey].IntValue;
        if (CombatState != null && TargetType == TargetType.AllEnemies)
        {
            await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(CombatState).WithHitCount(hits).Execute(choiceContext);
            await PowerCmd.Apply<Powers.ScratchAmpPower>(Owner.Creature, amp, Owner.Creature, this);
            return;
        }
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);
        await PowerCmd.Apply<Powers.ScratchAmpPower>(Owner.Creature, amp, Owner.Creature, this);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(DamageKey, 2m, ValueProp.Move),
        new IntVar(HitsKey, 2),
        new IntVar(ScratchAmpKey, 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Powers.ScratchAmpPower>()];

    protected override void OnUpgrade()
    {
        DynamicVars[ScratchAmpKey].UpgradeValueBy(1m);
    }
}







