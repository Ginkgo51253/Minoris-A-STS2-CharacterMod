
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 回合结束返还力量
能力英文名称: Revert Strength At Turn End
能力描述(ZHS): 在回合结束时，获得[blue]{Amount}[/blue]点力量。然后移除。
能力描述(ENG): At end of turn, gain [blue]{Amount}[/blue] Strength. Then remove this.
相关卡牌（本地键）: MINORIS-CARD031_MISDIRECT
*/
public class RevertStrengthAtTurnEndPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        if (Amount > 0)
        {
            await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
        }
        RemoveInternal();
    }
}














