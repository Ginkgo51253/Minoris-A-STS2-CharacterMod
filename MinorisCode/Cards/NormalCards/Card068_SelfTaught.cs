
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD068_SELF_TAUGHT
中文名称: 自学成才
英文名称: Self-Taught
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 1
卡牌效果: 获得1点力量、1点敏捷。在你的回合开始时额外抽1张牌
卡牌描述(ZHS): 获得1点力量、1点敏捷。在你的回合开始时额外抽1张牌
卡牌描述(ENG): Gain 1 Strength and 1 Dexterity. At the start of your turn, draw 1 additional card
升级效果: 获得固有
*/
public class Card068_SelfTaught() : MinorisCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(Owner.Creature, 1, Owner.Creature, this);
        await PowerCmd.Apply<Powers.SelfTaughtDrawPower>(Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}









