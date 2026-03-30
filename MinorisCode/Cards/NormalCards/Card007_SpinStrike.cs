
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD007_SPIN_STRIKE
中文名称: 回转打击
英文名称: Spin Strike
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: CardTag.Strike
费用: 1
卡牌效果: 造成 {Damage:diff()} 点伤害。你的下一张技能牌耗能减少 1 点。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。你的下一张技能牌耗能减少 1 点。
卡牌描述(ENG): Deal {Damage:diff()} damage. Your next Skill costs 1 less.
升级效果: 伤害+4
*/
public class Card007_SpinStrike() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await PowerCmd.Apply<Powers.NextSkillCostReducePower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}









