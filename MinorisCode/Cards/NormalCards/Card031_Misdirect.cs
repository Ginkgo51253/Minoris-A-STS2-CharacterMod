
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD031_MISDIRECT
中文名称: 误导
英文名称: Misdirect
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 0
卡牌效果: 使一名敌人在本回合失去 {LoseStrength:diff()} 点力量。
卡牌描述(ZHS): 使一名敌人在本回合失去 {LoseStrength:diff()} 点力量。
卡牌描述(ENG): Target loses {LoseStrength:diff()} Strength this turn.
升级效果: 获得保留
*/
public class Card031_Misdirect() : MinorisCard(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
         HoverTipFactory.FromKeyword(CardKeyword.Retain),
         HoverTipFactory.FromPower<StrengthPower>()];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("LoseStrength", 6)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        var lose = DynamicVars["LoseStrength"].IntValue;
        //await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -lose, Owner.Creature, this);
        await PowerCmd.Apply<Powers.MisdirectPower>(cardPlay.Target, lose, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}









