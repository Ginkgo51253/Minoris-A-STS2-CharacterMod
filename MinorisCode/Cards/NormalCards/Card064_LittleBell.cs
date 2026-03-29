
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD064_LITTLE_BELL
中文名称: 小铃铛
英文名称: Little Bell
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 在你的回合中，每当你失去生命时，获得1点力量
卡牌描述(ZHS): 在你的回合中，每当你失去生命时，获得1点力量
卡牌描述(ENG): During your turn, whenever you lose HP, gain 1 Strength
升级效果: 获得固有
*/
public class Card064_LittleBell() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.LittleBellPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}









