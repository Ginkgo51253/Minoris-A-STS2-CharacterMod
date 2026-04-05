
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 沙暴
能力英文名称: Sandstorm
能力描述(ZHS): 本回合中，你受到的伤害减半。回合结束时移除。
能力描述(ENG): This turn, you take half damage. Remove at end of turn.
相关卡牌（本地键）: MINORIS-CARD069_1_SANDSTORM
*/
public class SandstormPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-SANDSTORM").ToString() ?? string.Empty), isSmart: false)
    ];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        return target == Owner ? 0.5m : 1m;
    }
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await PowerCmd.TickDownDuration(this);
        await Task.CompletedTask;
    }
}














