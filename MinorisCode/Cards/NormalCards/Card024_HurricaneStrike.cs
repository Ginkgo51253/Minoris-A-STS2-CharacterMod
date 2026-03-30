
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD024_HURRICANE_STRIKE
中文名称: 飓风打击
英文名称: Hurricane Strike
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: CardTag.Strike
费用: 0
卡牌效果: 对所有敌人造成 {Damage:diff()} 点伤害 {HitsPerX:diff()}X 次。
卡牌描述(ZHS): 对所有敌人造成 {Damage:diff()} 点伤害 {HitsPerX:diff()}X 次。
卡牌描述(ENG): Deal {Damage:diff()} damage to ALL enemies {HitsPerX:diff()}X times.
升级效果: 对所有敌人造成{Damage:diff()}点伤害X次+1
*/
public class Card024_HurricaneStrike() : MinorisCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    private const string HitsPerXKey = "HitsPerX";

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new IntVar(HitsPerXKey, 2)
    ];

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var x = ResolveEnergyXValue();
        var hits = DynamicVars[HitsPerXKey].IntValue * x;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitCount(hits)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsPerXKey].UpgradeValueBy(1m);
    }
}









