
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD058_GATHER_STRENGTH
中文名称: 蓄势待发
英文名称: Buildup
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Rare
tag标签: 
费用: 3
卡牌效果: 获得 {Block:diff()} 点格挡。获得 {StrengthPower:diff()} 点力量。在你的下回合开始时，额外抽 {DrawExtraNextTurnPower:diff()} 张牌。保留你的手牌。结束你的回合。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。获得 {StrengthPower:diff()} 点力量。在你的下回合开始时，额外抽 {DrawExtraNextTurnPower:diff()} 张牌。保留你的手牌。结束你的回合。
卡牌描述(ENG): Gain {Block:diff()} Block. Gain {StrengthPower:diff()} Strength. Next turn, draw {DrawExtraNextTurnPower:diff()} additional cards. Retain your hand. End your turn.
升级效果: 格挡+8；获得点力量+1；在你的下回合开始时，额外抽张牌+1
*/
public class Card058_GatherStrength() : MinorisCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override HashSet<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(16m, ValueProp.Move),
        new PowerVar<StrengthPower>(2),
        new PowerVar<Powers.DrawExtraNextTurnPower>(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["StrengthPower"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<Powers.DrawExtraNextTurnPower>(Owner.Creature, DynamicVars["DrawExtraNextTurnPower"].IntValue, Owner.Creature, this);
        var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
        foreach (var c in hand) c.GiveSingleTurnRetain();
        PlayerCmd.EndTurn(Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(8m);
        DynamicVars["StrengthPower"].UpgradeValueBy(1m);
        DynamicVars["DrawExtraNextTurnPower"].UpgradeValueBy(1m);
    }
}









