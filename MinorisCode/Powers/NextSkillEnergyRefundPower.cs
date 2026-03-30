
namespace Minoris.MinorisCode.Powers;


/*
能力中文名称: 技能返能
能力英文名称: Skill Energy Refund
能力描述(ZHS): 本回合中，你下一张技能牌打出后获得1点能量。然后移除。回合结束时移除。
能力描述(ENG): This turn, the next Skill you play gives you 1 Energy. Then remove this. Remove at end of turn.
相关卡牌（本地键）: 
*/
public class NextSkillEnergyRefundPower : MinorisPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var playedCard = cardPlay.Card;
        if (playedCard.Owner != Owner.Player) return;
        if (playedCard.Type != CardType.Skill) return;
        Owner.Player?.PlayerCombatState?.GainEnergy(1);
        RemoveInternal();
        await Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side != side) return;
        RemoveInternal();
        await Task.CompletedTask;
    }
}














