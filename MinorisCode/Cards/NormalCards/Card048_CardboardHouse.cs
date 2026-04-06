
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD048_CARDBOARD_HOUSE
中文名称: 纸壳屋
英文名称: Cardboard House
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Uncommon
tag标签: 
费用: 1
卡牌效果: 获得 18 点格挡。本场战斗中每打出过一次本卡，下一次打出本卡时减少获得 3 点格挡（至少降至 0）。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。本场战斗中每打出过一次本卡，下一次减少获得 {Decrease:diff()} 点格挡（至少降至 0）。
卡牌描述(ENG): Gain {Block:diff()} Block. Each time you've played this card this combat, the next time reduces Block by {Decrease:diff()} (down to 0).
升级效果: 将减少值改为 2
*/
public class Card048_CardboardHouse() : MinorisCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool ShouldReceiveCombatHooks => true;
    private int _timesPlayedThisCombat;
    private decimal _accumulatedReduction;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(18m, ValueProp.Move),
        new IntVar("Decrease", 3)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        DynamicVars.Block.BaseValue -= DynamicVars["Decrease"].BaseValue;
        if (DynamicVars.Block.BaseValue < 0) DynamicVars.Block.BaseValue = 0;
        _accumulatedReduction += DynamicVars["Decrease"].BaseValue;
        _timesPlayedThisCombat++;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        _timesPlayedThisCombat = 0;
        DynamicVars.Block.BaseValue += _accumulatedReduction;
        _accumulatedReduction = 0;
        await Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Block.BaseValue -= _accumulatedReduction;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Decrease"].UpgradeValueBy(-1m);
    }
}



