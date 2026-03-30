
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD036_CAT_STEP
中文名称: 猫步
英文名称: Cat Step
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 2
卡牌效果: 获得3层滑溜
卡牌描述(ZHS): 获得3层滑溜
卡牌描述(ENG): Gain 3 Slippery
升级效果: 费用-1
*/
public class Card036_CatStep() : MinorisCard(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SlipperyPower>(Owner.Creature, 3, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}









