
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD074_WILD_SCRATCH
中文名称: 野性抓挠
英文名称: Wild Scratch
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Ancient
tag标签: 
费用: 0
卡牌效果: 对所有敌人造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。
卡牌描述(ZHS): 对所有敌人造成 {Damage:diff()} 点伤害 {Hits:diff()} 次。
卡牌描述(ENG): Deal {Damage:diff()} damage {Hits:diff()} times to ALL enemies.
升级效果: 对所有敌人造成{Damage:diff()}点伤害次+2
*/
public class Card074_WildScratch() : MinorisCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    private const string HitsKey = "Hits";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3m, ValueProp.Move),
        new IntVar(HitsKey, 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var hits = DynamicVars[HitsKey].IntValue;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .WithHitCount(hits)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[HitsKey].UpgradeValueBy(2m);
    }
}









