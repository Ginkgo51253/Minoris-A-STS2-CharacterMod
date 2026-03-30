
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 猫抓板
能力英文名称: Cat Scratch Board
能力描述(ZHS): 每当你打出一张技能牌时，临时获得[blue]{Amount}[/blue]点敏捷。
能力描述(ENG): Whenever you play a Skill, gain [blue]{Amount}[/blue] Dexterity this turn.
相关卡牌（本地键）: MINORIS-CARD063_CAT_SCRATCH_BOARD
*/
public class CatScratchBoardPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Skill) return;
        await PowerCmd.Apply<CatScratchBoardTemporaryDexterityPower>(Owner, Amount, Owner, null);
    }
}














