
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD047_SMALL_FREEZE_DRIED
中文名称: 小块冻干
英文名称: Small Freeze-Dried
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 获得4点活力
卡牌描述(ZHS): 获得4点活力
卡牌描述(ENG): Gain 4 Vigor
升级效果: 费用-1
*/
public class Card047_SmallFreezeDried() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(Owner.Creature, 4, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}









