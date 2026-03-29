
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD048_CARDBOARD_HOUSE
中文名称: 纸壳屋
英文名称: Cardboard House
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 获得（本回合已打出攻击牌数量 × {Block:diff()}）点格挡。
卡牌描述(ZHS): 获得（本回合已打出攻击牌数量 × {Block:diff()}）点格挡。
卡牌描述(ENG): Gain (Attacks played this turn × {Block:diff()}) Block.
升级效果: 格挡+1
*/
public class Card048_CardboardHouse() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(2m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combatState = Owner?.Creature?.CombatState;
        var attacksPlayedThisTurn = combatState == null
            ? 0
            : CombatManager.Instance.History.CardPlaysStarted.Count(e =>
                e.HappenedThisTurn(combatState)
                && e.CardPlay.Card.Owner == Owner
                && e.CardPlay.Card.Type == CardType.Attack);

        var blockToGain = attacksPlayedThisTurn * DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, blockToGain, ValueProp.Move, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}









