
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD013_AMBUSH
中文名称: 偷袭
英文名称: Ambush
卡牌类型: CardType.Attack
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 2
卡牌效果: 造成18点伤害。打出你手牌中的一张“抓挠”攻击牌
卡牌描述(ZHS): 造成18点伤害。打出你手牌中的一张“抓挠”攻击牌
卡牌描述(ENG): Deal 18 damage. Play a "Scratch" Attack in your hand
升级效果: 伤害+6
*/
public class Card013_Ambush() : MinorisCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
        if (CombatState == null) return;
        var hand = PileType.Hand.GetPile(Owner);
        var scratchInHand = hand.Cards.FirstOrDefault(c => c.Type == CardType.Attack && c.GetType().Name.Contains("Scratch"));
        if (scratchInHand != null)
        {
            await CardCmd.AutoPlay(choiceContext, scratchInHand, cardPlay.Target ?? Owner.Creature);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}









