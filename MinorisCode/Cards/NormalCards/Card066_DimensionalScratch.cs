
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD066_DIMENSIONAL_SCRATCH
中文名称: 次元抓
英文名称: Dimensional Scratch
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 2
卡牌效果: 你的“抓挠”攻击牌改为对所有敌人生效
卡牌描述(ZHS): 你的“抓挠”攻击牌改为对所有敌人生效
卡牌描述(ENG): Your "Scratch" Attacks now target all enemies
升级效果: 费用-1
*/
public class Card066_DimensionalScratch() : MinorisCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.DimensionalScratchPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}









