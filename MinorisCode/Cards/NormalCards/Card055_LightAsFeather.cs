
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD055_LIGHT_AS_FEATHER
中文名称: 轻于鸿毛
英文名称: Light as a Feather
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 1
卡牌效果: 给予 1 层缓慢。
卡牌描述(ZHS): 给予 1 层缓慢。
卡牌描述(ENG): Apply 1 Slow.
升级效果: 费用-1
*/
public class Card055_LightAsFeather() : MinorisCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SlowPower>(1)];
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
         HoverTipFactory.FromPower<SlowPower>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await PowerCmd.Apply<SlowPower>(cardPlay.Target, DynamicVars["SlowPower"].IntValue, Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}








