
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD010_QUADRACUT
中文名称: 四方斩
英文名称: Quadracut
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 对所有敌人造成 {Damage:diff()} 点伤害。这张牌会回到你的手牌。
卡牌描述(ZHS): 对所有敌人造成 {Damage:diff()} 点伤害。这张牌会回到你的手牌。
卡牌描述(ENG): Deal {Damage:diff()} damage to ALL enemies. Return this to your hand.
升级效果: 伤害+3
*/
public class Card010_Quadracut() : MinorisCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
        await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}









