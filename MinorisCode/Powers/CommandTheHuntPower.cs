
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 号令狂猎
能力英文名称: Command the Hunt
能力描述(ZHS): 本回合中，每当另一名玩家打出一张攻击牌时，额外打出[blue]{Amount}[/blue]次。回合结束时移除。
能力描述(ENG): This turn, whenever another player plays an Attack, play it [blue]{Amount}[/blue] additional time(s). Remove at end of turn.
相关卡牌（本地键）: MINORIS-CARD076_COMMAND_THE_HUNT
*/
public class CommandTheHuntPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => true;
    private bool _echoing;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (CombatState == null) return;
        if (playedCard.Owner == Owner.Player) return;
        if (playedCard.Type != CardType.Attack) return;
        if (_echoing) return;
        
        _echoing = true;
        for (var i = 0; i < Amount; i++)
        {
            var copy = CombatState.CreateCard(playedCard.CanonicalInstance, playedCard.Owner);
            copy.EnergyCost.SetThisCombat(0);
            copy.AddKeyword(CardKeyword.Exhaust);
            await CardCmd.AutoPlay(choiceContext, copy, cardPlay.Target);
        }
        _echoing = false;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await PowerCmd.TickDownDuration(this);
        await Task.CompletedTask;
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        return Task.CompletedTask;
    }
}














