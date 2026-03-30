
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 自学成才
能力英文名称: Self-Taught
能力描述(ZHS): 在你的回合开始时，额外抽[blue]{Amount}[/blue]张牌。
能力描述(ENG): At the start of your turn, draw [blue]{Amount}[/blue] additional card(s).
相关卡牌（本地键）: MINORIS-CARD068_SELF_TAUGHT
*/
public class SelfTaughtDrawPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}














