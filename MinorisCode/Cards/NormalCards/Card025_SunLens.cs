
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD025_SUN_LENS
中文名称: 阳光透镜
英文名称: Sun Lens
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 0
卡牌效果: 对所有敌人造成 {Damage:diff()} 点伤害。若消灭了敌人，获得 1 点能量。
卡牌描述(ZHS): 对所有敌人造成 {Damage:diff()} 点伤害。若消灭了敌人，获得 1 点能量。
卡牌描述(ENG): Deal {Damage:diff()} damage to ALL enemies. If an enemy is killed, gain 1 Energy.
升级效果: 伤害+3
*/
public class Card025_SunLens() : MinorisCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var enemies = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsAlive).ToList();
        var before = enemies.ToDictionary(e => e, e => e.CurrentHp);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
        var afterKills = enemies.Any(e => !e.IsAlive || e.CurrentHp <= 0);
        if (afterKills) Owner.PlayerCombatState!.GainEnergy(1);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}









