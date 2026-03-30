
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 致命咖啡
能力英文名称: Deadly Coffee
能力描述(ZHS): 在你的回合结束时，失去[blue]{Amount}[/blue]点生命。然后移除。
能力描述(ENG): At the end of your turn, lose [blue]{Amount}[/blue] HP. Then remove this.
相关卡牌（本地键）: MINORIS-CARD041_DEADLY_COFFEE
*/
public class DeadlyCoffeePower : MinorisPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        if (Amount > 0)
        {
            await CreatureCmd.Damage(choiceContext, new[] { Owner }, Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        }
        RemoveInternal();
    }
}














