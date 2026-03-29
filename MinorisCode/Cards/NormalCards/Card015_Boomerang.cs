
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD015_BOOMERANG
中文名称: 回旋镖
英文名称: Boomerang
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 0
卡牌效果: 造成5点伤害。打出后将本牌置于你的抽牌堆顶部
卡牌描述(ZHS): 造成5点伤害。打出后将本牌置于你的抽牌堆顶部
卡牌描述(ENG): Deal 5 damage. Put this on top of your draw pile
升级效果: 伤害+3
*/
public class Card015_Boomerang() : MinorisCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Top);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}









