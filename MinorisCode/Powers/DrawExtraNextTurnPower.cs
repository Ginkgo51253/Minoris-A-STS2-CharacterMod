
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 下回合额外抽牌
能力英文名称: Draw Extra Next Turn
能力描述(ZHS): 在你的下回合开始时，额外抽[blue]{Amount}[/blue]张牌。然后移除。
能力描述(ENG): At the start of your next turn, draw [blue]{Amount}[/blue] additional card(s). Then remove this.
相关卡牌（本地键）: MINORIS-CARD058_GATHER_STRENGTH
*/
public class DrawExtraNextTurnPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        if (Amount > 0 && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player, fromHandDraw: true);
        }
        RemoveInternal();
        await Task.CompletedTask;
    }
}














