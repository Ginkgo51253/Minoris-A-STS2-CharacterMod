
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD048_CARDBOARD_HOUSE
中文名称: 纸壳屋
英文名称: Cardboard House
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 本回合每打出1张攻击牌获得 2 点格挡（共 X 点）。
卡牌描述(ZHS): 本回合每打出1张攻击牌获得 {Block:diff()} 点格挡（共 {TotalBlockFinal:diff()} 点）。
卡牌描述(ENG): Gain {Block:diff()} Block per attack played this turn ({TotalBlockFinal:diff()} total).
升级效果: 格挡+1
*/
public class Card048_CardboardHouse() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new BlockVar(2m, ValueProp.Move),
            new CalculationBaseVar(0m),
            new CalculationExtraVar(1m),
            new CalculatedVar("TotalBlockFinal").WithMultiplier((CardModel card, Creature? _) =>
            {
                var attacksPlayedThisTurn = ((Card048_CardboardHouse)card).GetAttacksPlayedThisTurn();
                var blockPerAttack = card.DynamicVars.Block.BaseValue;
                var baseBlock = attacksPlayedThisTurn * blockPerAttack;
                var frailAmount = card.Owner?.Creature?.GetPowerAmount<FrailPower>() ?? 0;
                var frailMultiplier = frailAmount > 0 ? (decimal)Math.Pow(0.75, frailAmount) : 1m;
                return (int)(baseBlock * frailMultiplier);
            })
        ];
    
    private int GetAttacksPlayedThisTurn()
    {
        var combatState = Owner?.Creature?.CombatState;
        return combatState == null
            ? 0
            : CombatManager.Instance.History.CardPlaysStarted.Count(e =>
                e.HappenedThisTurn(combatState)
                && e.CardPlay.Card.Owner == Owner
                && e.CardPlay.Card.Type == CardType.Attack);
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var attacksPlayedThisTurn = GetAttacksPlayedThisTurn();
            var blockPerAttack = DynamicVars.Block.BaseValue;
            var baseBlock = attacksPlayedThisTurn * blockPerAttack;
            
            var frailAmount = Owner?.Creature?.GetPowerAmount<FrailPower>() ?? 0;
            var frailMultiplier = frailAmount > 0 ? (decimal)Math.Pow(0.75, frailAmount) : 1m;
            var finalBlock = (int)(baseBlock * frailMultiplier);
            
            var frailText = frailAmount > 0 ? $"\n当前脆弱层数: {frailAmount} (-{(int)((1 - frailMultiplier) * 100)}%)" : "";
            var description = $"本回合已打出攻击牌: {attacksPlayedThisTurn}\n每张攻击牌格挡: {blockPerAttack}\n基础格挡总计: {baseBlock}{frailText}\n最终格挡: {finalBlock}";
            return [new HoverTip(TitleLocString, description)];
        }
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attacksPlayedThisTurn = GetAttacksPlayedThisTurn();
        var blockToGain = attacksPlayedThisTurn * DynamicVars.Block.BaseValue;
        var frailAmount = Owner.Creature.GetPowerAmount<FrailPower>();
        var frailMultiplier = frailAmount > 0 ? (decimal)Math.Pow(0.75, frailAmount) : 1m;
        blockToGain = (int)(blockToGain * frailMultiplier);
        await CreatureCmd.GainBlock(Owner.Creature, blockToGain, ValueProp.Move, cardPlay);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}








