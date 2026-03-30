
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 玩具猫爪
能力英文名称: Toy Claw
能力描述(ZHS): 在你每回合开始时，将一张“抓挠”置入你的手牌。若层数不少于2，则生成升级过的“抓挠”。
能力描述(ENG): At the start of your turn, add a "Scratch" to your hand. If stacks are 2 or more, add an upgraded "Scratch" instead.
相关卡牌（本地键）: MINORIS-CARD062_TOY_CLAW
*/
public class ToyClawPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || CombatState == null) return;
        var scratch = CombatState.CreateCard<Minoris.MinorisCode.Cards.Card062_1_Scratch>(Owner.Player);
        if (Amount >= 2) CardCmd.Upgrade(scratch);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(scratch, PileType.Hand, false, CardPilePosition.Top));
    }
}














