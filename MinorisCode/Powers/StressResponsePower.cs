
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 应激反应
能力英文名称: Stress Response
能力描述(ZHS): 本回合中，每当你打出一张牌时，失去1点生命并获得[blue]{Amount}[/blue]点格挡。回合结束时移除。
能力描述(ENG): This turn, whenever you play a card, lose 1 HP and gain [blue]{Amount}[/blue] Block. Remove at end of turn.
相关卡牌（本地键）: MINORIS-CARD049_STRESS_RESPONSE
*/
public class StressResponsePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-STRESS_RESPONSE").ToString() ?? string.Empty)
            .Replace("{Amount}", Amount.ToString()), isSmart: false)
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        await CreatureCmd.Damage(choiceContext, Owner, 1m, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, cardPlay);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}













