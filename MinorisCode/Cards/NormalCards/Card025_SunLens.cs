
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD025_SUN_LENS
中文名称: 阳光透镜
英文名称: Sun Lens
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 0
卡牌效果: 对所有敌人造成 {Damage:diff()} 点伤害。每消灭一个敌人，抽 1 张牌并获得 1 点能量。
卡牌描述(ZHS): 对所有敌人造成 {Damage:diff()} 点伤害。每消灭一个敌人，抽 1 张牌并获得 1 点能量。
卡牌描述(ENG): Deal {Damage:diff()} damage to ALL enemies. For each enemy killed, draw 1 card and gain 1 Energy.
升级效果: 伤害+3
*/
public class Card025_SunLens() : MinorisCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11m, ValueProp.Move), new EnergyVar(1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        var enemies = CombatState.GetOpponentsOf(Owner.Creature).Where(e => e.IsAlive).ToList();
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
        if (enemies.Count == 0) return;
        var kills = enemies.Count(e => !e.IsAlive || e.CurrentHp <= 0);
        if (kills <= 0) return;
        await CardPileCmd.Draw(choiceContext, kills, Owner);
        Owner.PlayerCombatState!.GainEnergy(kills);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}







