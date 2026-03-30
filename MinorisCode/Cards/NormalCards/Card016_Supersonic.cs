
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD016_SUPERSONIC
中文名称: 超越音速
英文名称: Supersonic
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 0
卡牌效果: 失去 1 点生命。造成 {Damage:diff()} 点伤害。抽 1 张牌。
卡牌描述(ZHS): 失去 1 点生命。造成 {Damage:diff()} 点伤害。抽 1 张牌。
卡牌描述(ENG): Lose 1 HP. Deal {Damage:diff()} damage. Draw 1 card.
升级效果: 伤害+2
*/
public class Card016_Supersonic() : MinorisCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, Owner.Creature, 1m, ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this);
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}









