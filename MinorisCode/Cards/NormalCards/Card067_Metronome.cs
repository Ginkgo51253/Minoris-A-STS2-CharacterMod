
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD067_METRONOME
中文名称: 节拍器
英文名称: Metronome
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 3
卡牌效果: 当你打出一张打击后，抽一张牌
卡牌描述(ZHS): 当你打出一张打击后，抽一张牌
卡牌描述(ENG): Whenever you play a Strike, draw 1 card
升级效果: 费用-1
*/
public class Card067_Metronome() : MinorisCard(3, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.MetronomePower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}









