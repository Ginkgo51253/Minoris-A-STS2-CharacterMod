
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD001_STRIKE
中文名称: 打击
英文名称: Strike
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Basic
tag标签: CardTag.Strike
费用: 1
卡牌效果: 造成 {Damage:diff()} 点伤害。
卡牌描述(ZHS): 造成 {Damage:diff()} 点伤害。
卡牌描述(ENG): Deal {Damage:diff()} damage.
升级效果: 伤害+3
*/
public class Card001_Strike() : MinorisCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}










