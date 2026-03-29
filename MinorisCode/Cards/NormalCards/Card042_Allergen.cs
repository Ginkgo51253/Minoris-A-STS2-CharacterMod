
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD042_ALLERGEN
中文名称: 过敏原
英文名称: Allergen
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 0
卡牌效果: 本回合中，当你打出攻击牌时，抽 1 张牌，失去 2 点生命。
卡牌描述(ZHS): 本回合中，当你打出攻击牌时，抽 1 张牌，失去 2 点生命。
卡牌描述(ENG): This turn, whenever you play an Attack, draw 1 card and lose 2 HP.
升级效果: 获得保留
*/
public class Card042_Allergen() : MinorisCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.AllergenPower>(Owner.Creature, 1, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}









