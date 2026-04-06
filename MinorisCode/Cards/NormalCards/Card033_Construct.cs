﻿﻿﻿
namespace Minoris.MinorisCode.Cards;


/*
本地键: MINORIS-CARD033_CONSTRUCT
中文名称: 构筑
英文名称: Construct
卡牌类型: CardType.Skill
卡牌稀有度: CardRarity.Common
tag标签: 
费用: 1
卡牌效果: 获得 {Block:diff()} 点格挡。在战斗中，你每打出一次本卡，下一次打出时额外获得 {BonusPerPlay:diff()} 点格挡。
卡牌描述(ZHS): 获得 {Block:diff()} 点格挡。在战斗中，你每打出一次本卡，下一次打出时额外获得 {BonusPerPlay:diff()} 点格挡。
卡牌描述(ENG): Gain {Block:diff()} Block. Each time you play this in combat, gain +{BonusPerPlay:diff()} Block the next time you play it this combat.
升级效果: 在战斗中，你每打出一次本卡，下一次打出时额外获得格挡+1
*/
public class Card033_Construct() : MinorisCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const string BonusPerPlayKey = "BonusPerPlay";

    public override bool ShouldReceiveCombatHooks => true;
    private int _timesPlayedThisCombat;
    private decimal _accumulatedBlock;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new IntVar(BonusPerPlayKey, 1)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Move, cardPlay);
        DynamicVars.Block.BaseValue += DynamicVars[BonusPerPlayKey].BaseValue;
        _accumulatedBlock += DynamicVars[BonusPerPlayKey].BaseValue;
        _timesPlayedThisCombat++;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        _timesPlayedThisCombat = 0;
        DynamicVars.Block.BaseValue -= _accumulatedBlock;
        _accumulatedBlock = 0;
        await Task.CompletedTask;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Block.BaseValue += _accumulatedBlock;
    }

    protected override void OnUpgrade()
    {
        DynamicVars[BonusPerPlayKey].UpgradeValueBy(1m);
    }
}









