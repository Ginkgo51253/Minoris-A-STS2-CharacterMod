
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD034_ROLL
中文名称: 翻滚
英文名称: Roll
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 获得15点格挡。将两张晕眩置入你的抽牌堆
卡牌描述(ZHS): 获得15点格挡。将两张晕眩置入你的抽牌堆
卡牌描述(ENG): Gain 15 Block. Shuffle 2 Dazed into your draw pile
升级效果: 格挡+5
*/
public class Card034_Roll() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15m, ValueProp.Move)];
    private int _dazed = 2;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        if (CombatState == null) return;
        for (var i = 0; i < _dazed; i++)
        {
            var d = CombatState.CreateCard<Dazed>(Owner);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(d, PileType.Draw, true, CardPilePosition.Random));
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}









