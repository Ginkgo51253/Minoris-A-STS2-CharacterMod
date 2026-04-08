namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 玩具猫爪
能力英文名称: Toy Claw
能力描述(ZHS): 在你每回合开始时，将一张"抓挠"置入你的手牌
能力描述(ENG): At the start of your turn, add a "Scratch" to your hand.
相关卡牌（本地键）: MINORIS-CARD062_TOY_CLAW
*/
public class ToyClawPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        new HoverTip(this, (new LocString("HoverTip", "MINORIS-HOVERTIP-TOY_CLAW").ToString() ?? string.Empty), isSmart: false)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || CombatState == null) return;
        Flash();
        for (int i = 0; i < Amount; i++)
        {
            var scratch = CombatState.CreateCard<Minoris.MinorisCode.Cards.Card062_1_Scratch>(Owner.Player);
            await CardPileCmd.AddGeneratedCardToCombat(scratch, PileType.Hand, true, CardPilePosition.Top);
        }
    }
}
