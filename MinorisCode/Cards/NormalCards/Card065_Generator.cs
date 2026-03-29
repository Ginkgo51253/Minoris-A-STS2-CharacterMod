
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD065_GENERATOR
中文名称: 发电机
英文名称: Generator
卡牌类型: CardType.Power
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 2
卡牌效果: 在你的回合中，你每打出一张攻击牌，下一回合就会多获得 1 点能量。
卡牌描述(ZHS): 在你的回合中，你每打出一张攻击牌，下一回合就会多获得 1 点能量。
卡牌描述(ENG): During your turn, whenever you play an Attack, gain 1 additional Energy next turn.
升级效果: 移除虚无
*/
public class Card065_Generator() : MinorisCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.GeneratorPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}









