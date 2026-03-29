
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD063_CAT_SCRATCH_BOARD
中文名称: 猫抓板
英文名称: Scratch Board
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 你每打出一张技能牌，临时获得2点敏捷
卡牌描述(ZHS): 你每打出一张技能牌，临时获得2点敏捷
卡牌描述(ENG): Whenever you play a Skill, gain 2 Dexterity this turn
升级效果: 获得固有
*/
public class Card063_CatScratchBoard() : MinorisCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.CatScratchBoardPower>(Owner.Creature, 2, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}









