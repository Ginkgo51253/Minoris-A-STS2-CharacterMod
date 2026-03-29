
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 节拍器
能力英文名称: Metronome
能力描述(ZHS): 当你打出一张打击牌时，抽1张牌。
能力描述(ENG): Whenever you play a Strike, draw 1 card.
相关卡牌（本地键）: MINORIS-CARD067_METRONOME
*/
public class MetronomePower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (!playedCard.GetType().Name.Contains("Strike")) return;
        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}














