using Minoris.MinorisCode.Powers;
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD044_MINT_CANDY
中文名称: 薄荷糖
英文名称: Mint Candy
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 抽 {Cards:diff()} 张牌。你的下一张攻击牌耗能减少 1 点。
卡牌描述(ZHS): 抽 {Cards:diff()} 张牌。你的下一张攻击牌耗能减少 1 点。
卡牌描述(ENG): Draw {Cards:diff()} cards. Your next Attack costs 1 less.
升级效果: Cards+1
*/
public class Card044_MintCandy() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    private const string EnergyReduceKey = "EnergyReduce";

    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(1),
        new IntVar(EnergyReduceKey, 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await PowerCmd.Apply<Powers.NextAttackCostReducePower>(Owner.Creature, DynamicVars[EnergyReduceKey].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}









